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
        public string ajouterclients(string nom, string prenom, string email, string pass)
        {
            try
            {
                var client = new Client();
                client.nom = nom;
                client.prenom = prenom;
                client.email = email;
                client.password = pass;
                dbContext.Clients.Add(client);
                dbContext.SaveChanges();
                return "enregistrement ok";
            } catch (Exception ex)
            {
                return ex.ToString();
            }
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
