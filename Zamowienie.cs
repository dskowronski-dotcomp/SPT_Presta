// Decompiled with JetBrains decompiler
// Type: SPT_Presta.Zamowienie
// Assembly: spt_presta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3ED6FB4A-3869-4961-B24F-AC70524369F0
// Assembly location: C:\Program Files (x86)\Common Files\Soneta\Assemblies\spt_presta.dll

using System.Net;
using System.Xml;

namespace SPT_Presta
{
    internal class Zamowienie
    {
        public int IdCustomer { get; }
        /// <summary>
        ///  zmienne na potrzeby przetworzenia zamowienia
        /// </summary>
        private string[] ps_productId = new string[1000000];
        private int zmiennaProduktId = 0;
        private int[] product_id = new int[1000000];
        private string[] ps_product_quantity = new string[1000000];
        private int[] product_quantity = new int[1000000];
        private int zmiennaIloscProduktu = 0;
        private string ps_id_address_delivery;
        private int id_address_delivery;

        public Zamowienie(int order_id)
        {
            WebRequest webRequest = WebRequest.Create(TworzenieDokumentuSPTWorker.URLorders + order_id.ToString());
            webRequest.Credentials = (ICredentials)new NetworkCredential(TworzenieDokumentuSPTWorker.ps_login, "");
            using (WebResponse response = webRequest.GetResponse())
            {
                using (XmlReader xmlReader = XmlReader.Create(response.GetResponseStream()))
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            if (xmlReader.Name == "product_id")
                            {
                                xmlReader.Read();
                                ps_productId[zmiennaProduktId] = xmlReader.Value;
                                product_id[zmiennaProduktId] = int.Parse(ps_productId[zmiennaProduktId]);
                                ++zmiennaProduktId;
                            }
                            else if (xmlReader.Name == "product_quantity")
                            {
                                xmlReader.Read();
                                ps_product_quantity[zmiennaIloscProduktu] = xmlReader.Value;
                                product_quantity[zmiennaIloscProduktu] = int.Parse(ps_product_quantity[zmiennaIloscProduktu]);
                                ++zmiennaIloscProduktu;
                            }
                            else if (xmlReader.Name == "id_address_delivery")
                            {
                                xmlReader.Read();
                                ps_id_address_delivery = xmlReader.Value;
                                id_address_delivery = int.Parse(ps_id_address_delivery);
                            }
                            else if (xmlReader.Name == "id_customer")
                            {
                                xmlReader.Read();
                                this.IdCustomer = int.Parse(xmlReader.Value);
                            }
                        }
                    }
                }
            }
        }
    }
}
