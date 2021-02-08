// Decompiled with JetBrains decompiler
// Type: SPT_Presta.Customer
// Assembly: spt_presta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3ED6FB4A-3869-4961-B24F-AC70524369F0
// Assembly location: C:\Program Files (x86)\Common Files\Soneta\Assemblies\spt_presta.dll

using System;
using System.Net;
using System.Xml;

namespace SPT_Presta
{
  internal class Customer
  {
    public string imie { get; }

    public string nazwisko { get; }

    public string email { get; }

    public string company { get; }

    public string nip { get; }

    public Customer(int idCustomer)
    {
      WebRequest webRequest = WebRequest.Create("http://localhost/presta/api/customers/" + Convert.ToString(idCustomer));
      webRequest.Credentials = (ICredentials) new NetworkCredential(TworzenieDokumentuSPTWorker.ps_login, "");
      using (WebResponse response = webRequest.GetResponse())
      {
        using (XmlReader xmlReader = XmlReader.Create(response.GetResponseStream()))
        {
          while (xmlReader.Read())
          {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
              if (xmlReader.Name == "firstname")
              {
                xmlReader.Read();
                this.imie = xmlReader.Value;
              }
              else if (xmlReader.Name == "lastname")
              {
                xmlReader.Read();
                this.nazwisko = xmlReader.Value;
              }
              else if (xmlReader.Name == "email")
              {
                xmlReader.Read();
                this.email = xmlReader.Value;
              }
              else if (xmlReader.Name == "company")
              {
                xmlReader.Read();
                this.company = xmlReader.Value;
              }
              else if (xmlReader.Name == "siret")
              {
                xmlReader.Read();
                this.nip = xmlReader.Value;
              }
            }
          }
        }
      }
    }
  }
}
