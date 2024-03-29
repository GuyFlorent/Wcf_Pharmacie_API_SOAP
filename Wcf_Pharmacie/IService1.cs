﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Wcf_Pharmacie
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom d'interface "IService1" à la fois dans le code et le fichier de configuration.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);
        [OperationContract]
        string ajouterclients(Client client);
        [OperationContract]
        bool verifierClients(string email, string pass);
        [OperationContract]
        List<ProduitReturn> listeProduit();
        [OperationContract]
        string passerCommande(string nomclient, string nomproduit, int quantité);
        [OperationContract]
        ClientReturn recupereParEmail(string email);
        [OperationContract]
        string modifierClients(ClientReturn client);
        [OperationContract]
        string supprimerClients(ClientReturn client);
        [OperationContract]
        List<orderHisto> getcommandehisto(ClientReturn client);
        [OperationContract]
        List<InfoProduit> infoProduits();


        // TODO: ajoutez vos opérations de service ici
    }




    // Utilisez un contrat de données comme indiqué dans l'exemple ci-après pour ajouter les types composites aux opérations de service.

    [DataContract]
    public class InfoProduit
    {
        [DataMember]
        public string nom_produit { get; set; }
        [DataMember]
        public Nullable<decimal> prix_unite { get; set; }
    }


    [DataContract]
    public class orderHisto
    {
        [DataMember]
        public string heureCommand { get; set; }
        [DataMember]
        public string statutLivraison { get; set; }
        [DataMember]
        public int quantite { get; set; }
        [DataMember]

        public double prix_total { get; set; }
        [DataMember]

        public string nom_Produit { get; set; }
        [DataMember]
        public double prix_Produit_Unite { get; set; }
    }

    [DataContract]
    public class ClientReturn
    {
        [DataMember]
        public int id_client { get; set; }
        [DataMember]
        public string nom { get; set; }
        [DataMember]
        public string prenom { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string password { get; set; }
        [DataMember]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
      
        public virtual ICollection<Achat> Achats { get; set; }
        [DataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Produit> Produits { get; set; }
    }
    [DataContract]
    public class ProduitReturn
    {
        [DataMember]
        public int id_stock { get; set; }
        [DataMember]
        public byte[] image_Produit { get; set; }
        [DataMember]
        public string nom_produit_stock { get; set; }
        [DataMember]
        public string image_test { get; set; }
        [DataMember]
        public Nullable<int> quantite_produit { get; set; }
        [DataMember]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Achat> Achats { get; set; }
        [DataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        
        public virtual ICollection<Produit> Produits { get; set; }
    }
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
