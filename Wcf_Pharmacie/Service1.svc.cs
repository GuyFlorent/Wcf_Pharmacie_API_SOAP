using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Wcf_Pharmacie
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" dans le code, le fichier svc et le fichier de configuration.
    // REMARQUE : pour lancer le client test WCF afin de tester ce service, sélectionnez Service1.svc ou Service1.svc.cs dans l'Explorateur de solutions et démarrez le débogage.
    public class Service1 : IService1
    {
        PharmacieEthodetEntities dbContext = new PharmacieEthodetEntities();
        public string ajouterclients(Client client)
        {
            
                var liste = dbContext.Clients.ToList();
                var ajout = liste.FirstOrDefault(f => f.email == client.email);

                if (ajout == null )
                    {
                        dbContext.Clients.Add(client);
                        dbContext.SaveChanges();
                    
                    return client.nom + " enregistrement ok";

                } else
                return "email exixte deja !! \nVeillez saisir un autre s'il vous plait"; 
          
         
        }

        public List<orderHisto> getcommandehisto(ClientReturn client)
        {

            List<orderHisto> listOrderHisto = new List<orderHisto>();

            List<Commande> listCommand = dbContext.Commandes.Where(w => w.id_client == client.id_client).ToList();



            foreach(Commande com in listCommand)
            {
                orderHisto orderhisto = new orderHisto();

                Achat achat = dbContext.Achats.FirstOrDefault(f => f.id_commande == com.id_commande);

                Stock st = dbContext.Stocks.FirstOrDefault(f => f.id_stock == achat.id_stock);

                Produit pro = dbContext.Produits.FirstOrDefault(f => f.id_stock == st.id_stock);

                orderhisto.heureCommand = com.heure_commande;
                orderhisto.statutLivraison = com.statut_livraison;
                orderhisto.quantite = (int)achat.quantité;
                orderhisto.prix_total = (double)achat.prix_total;
                orderhisto.nom_Produit = st.nom_produit_stock;
                orderhisto.prix_Produit_Unite =(double) pro.prix_unite;
                listOrderHisto.Add(orderhisto);

            }

            return listOrderHisto;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public List<InfoProduit> infoProduits() // pour avoir le nom et le prix unité pour utilisé chez le client
        {
            List<InfoProduit> infoProduits = new List<InfoProduit>();
            List<Produit> produits = dbContext.Produits.ToList();
            foreach(Produit p in produits)
            {
                InfoProduit infos = new InfoProduit();
                infos.nom_produit = p.nom_produit;
                infos.prix_unite = p.prix_unite;
                infoProduits.Add(infos);
            } return infoProduits;
        }

        public List<ProduitReturn> listeProduit()
        {
            List<ProduitReturn> liste = new List<ProduitReturn>();
            List<Stock> prop = dbContext.Stocks.ToList();
            foreach(Stock p in prop)
            {
                ProduitReturn produit = new ProduitReturn();
                produit.nom_produit_stock = p.nom_produit_stock;
                produit.quantite_produit = p.quantite_produit;
                produit.id_stock = p.id_stock;
                produit.image_test = p.image_test;

              //  produit.image_Produit = p.image_Produit; // ajout de l'image mais trop lourd 
                liste.Add(produit);
            }
            return liste;
        }

      
        public string modifierClients(ClientReturn client)
        {
            try
            {
                var modif = dbContext.Clients.FirstOrDefault(f => f.id_client == client.id_client);
                modif.nom = client.nom;
                modif.prenom = client.prenom;
                modif.email = client.email;
                modif.password = client.password;
                dbContext.SaveChanges();
                return "modifié !";
            }catch(Exception e)
            {
                return "Erreur !!!";
            }
        }

        public string passerCommande(string nomclient, string nomproduit, int quantité)
        {
            var cli = dbContext.Clients.FirstOrDefault(f => f.nom == nomclient);
            var prod = dbContext.Stocks.FirstOrDefault(f => f.nom_produit_stock == nomproduit);
            var prixProd = dbContext.Produits.FirstOrDefault(f => f.nom_produit == nomproduit);
            if (prod.quantite_produit >= quantité) // si la quantité est inférieur ou égale a la quantité en stock
            {

                Commande nouvelCommande = new Commande(); // création nouvelle commande
                nouvelCommande.heure_commande = DateTime.Now.ToString();
                nouvelCommande.statut_commande = "Validé";
                nouvelCommande.statut_livraison = "Livré";
                nouvelCommande.id_client = cli.id_client;  // id_client dans commande sera egale au meme id_client dans client car le meme nom de client
                dbContext.Commandes.Add(nouvelCommande); //creation de la commande

                Achat nouvelAchat = new Achat();//reation nouvel achat
                nouvelAchat.quantité = quantité;
                nouvelAchat.id_stock = prod.id_stock; // id_stoc dans achat sera egale au meme id_stock dans produit car le meme nom de produit
                prod.quantite_produit -= quantité;
                nouvelAchat.prix_total = quantité * prixProd.prix_unite;
                dbContext.Achats.Add(nouvelAchat);


                dbContext.SaveChanges();

                var com = dbContext.Commandes.FirstOrDefault(f => f.id_commande == nouvelCommande.id_commande);
                var achat = dbContext.Achats.FirstOrDefault(f => f.id_achat == nouvelAchat.id_achat);
                achat.id_commande = com.id_commande; // recupération de id de la nouvelle commande

                dbContext.SaveChanges();
                return "commande ok";
            }
            else
                return"la quantité de produit est insuffissant  !!! \n verifier stock!";
        }

        public ClientReturn recupereParEmail(string email)
        {
            ClientReturn clinew = new ClientReturn();

            var cli = dbContext.Clients.FirstOrDefault(f => f.email == email);
            clinew.nom = cli.nom;
            clinew.prenom = cli.prenom;
            clinew.email = cli.email;
            clinew.password = cli.password;
            clinew.id_client = cli.id_client;

            return clinew;
        }

        public string supprimerClients(ClientReturn client)
        {
           try
            {
                var suprime = dbContext.Clients.FirstOrDefault(f => f.id_client == client.id_client);
                var comClientSup = dbContext.Commandes.Where(f => f.id_client == suprime.id_client);
                foreach (var a in comClientSup)
                {

                    dbContext.Commandes.Remove(a);

                    var achat = dbContext.Achats.FirstOrDefault(f => f.id_commande == a.id_commande);
                    dbContext.Achats.Remove(achat);
                }

                dbContext.Clients.Remove(suprime);
                dbContext.Clients.Remove(suprime);
                dbContext.SaveChanges();
                return "utilisateur supprimé de la BDD!";
            } catch ( Exception ex)
            {
                return "erreur !!";
            }
        }

        public bool verifierClients(string email, string pass)
        {
            var verifie = dbContext.Clients.FirstOrDefault(f => f.email == email && f.password == pass);
            if (verifie != null)
            {
                return true;
            }
            else return false;
        }

       
    }
}
