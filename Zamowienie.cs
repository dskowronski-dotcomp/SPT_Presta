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
    public int idCustomer { get; }

    public Zamowienie(int order_id)
    {
      WebRequest webRequest = WebRequest.Create("http://localhost/presta/api/orders/" + order_id.ToString());
      webRequest.Credentials = (ICredentials) new NetworkCredential(TworzenieDokumentuSPTWorker.ps_login, "");
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
                TworzenieDokumentuSPTWorker.ps_productId[TworzenieDokumentuSPTWorker.zmiennaProduktId] = xmlReader.Value;
                TworzenieDokumentuSPTWorker.product_id[TworzenieDokumentuSPTWorker.zmiennaProduktId] = int.Parse(TworzenieDokumentuSPTWorker.ps_productId[TworzenieDokumentuSPTWorker.zmiennaProduktId]);
                ++TworzenieDokumentuSPTWorker.zmiennaProduktId;
              }
              else if (xmlReader.Name == "product_quantity")
              {
                xmlReader.Read();
                TworzenieDokumentuSPTWorker.ps_product_quantity[TworzenieDokumentuSPTWorker.zmiennaIloscProduktu] = xmlReader.Value;
                TworzenieDokumentuSPTWorker.product_quantity[TworzenieDokumentuSPTWorker.zmiennaIloscProduktu] = int.Parse(TworzenieDokumentuSPTWorker.ps_product_quantity[TworzenieDokumentuSPTWorker.zmiennaIloscProduktu]);
                ++TworzenieDokumentuSPTWorker.zmiennaIloscProduktu;
              }
              else if (xmlReader.Name == "id_address_delivery")
              {
                xmlReader.Read();
                TworzenieDokumentuSPTWorker.ps_id_address_delivery = xmlReader.Value;
                TworzenieDokumentuSPTWorker.id_address_delivery = int.Parse(TworzenieDokumentuSPTWorker.ps_id_address_delivery);
              }
              else if (xmlReader.Name == "id_customer")
              {
                xmlReader.Read();
                this.idCustomer = int.Parse(xmlReader.Value);
              }
            }
          }
        }
      }
    }
  }
}
