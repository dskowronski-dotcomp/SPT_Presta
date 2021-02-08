// Decompiled with JetBrains decompiler
// Type: SPT_Presta.Faktura
// Assembly: spt_presta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3ED6FB4A-3869-4961-B24F-AC70524369F0
// Assembly location: C:\Program Files (x86)\Common Files\Soneta\Assemblies\spt_presta.dll

using Soneta.Types;
using System;
using System.Net;
using System.Xml;

namespace SPT_Presta
{
  internal class Faktura
  {
    private string ps_total_paid_tax_excl;
    private string ps_total_paid_tax_incl;
    private string ps_dateInvoice;

    public Decimal wartoscNetto { get; }

    public Decimal wartoscBrutto { get; }

    public Date dataFaktury { get; }

    public int number { get; }

    public int idOrder { get; }

    public Faktura(int idFaktury)
    {
      WebRequest webRequest = WebRequest.Create("http://localhost/presta/api/order_invoices/" + idFaktury.ToString());
      webRequest.Credentials = (ICredentials) new NetworkCredential("XK96WGNV1WXAXYFRBYI2HTF7CMV3PZIB", "");
      using (WebResponse response = webRequest.GetResponse())
      {
        using (XmlReader xmlReader = XmlReader.Create(response.GetResponseStream()))
        {
          while (xmlReader.Read())
          {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
              if (xmlReader.Name == "total_paid_tax_excl")
              {
                xmlReader.Read();
                this.ps_total_paid_tax_excl = xmlReader.Value;
                this.ps_total_paid_tax_excl = this.ps_total_paid_tax_excl.Substring(0, this.ps_total_paid_tax_excl.Length - 4);
                this.ps_total_paid_tax_excl = this.ps_total_paid_tax_excl.Replace(".", ",");
                this.wartoscNetto = Decimal.Parse("95,92");
              }
              else if (xmlReader.Name == "date_add")
              {
                xmlReader.Read();
                this.ps_dateInvoice = xmlReader.Value;
                this.ps_dateInvoice = this.ps_dateInvoice.Replace("-", "/");
                this.dataFaktury = (Date) Convert.ToDateTime(this.ps_dateInvoice);
              }
              else if (xmlReader.Name == "total_paid_tax_incl")
              {
                xmlReader.Read();
                this.ps_total_paid_tax_incl = xmlReader.Value;
                this.ps_total_paid_tax_incl = this.ps_total_paid_tax_incl.Substring(0, this.ps_total_paid_tax_incl.Length - 4);
                this.ps_total_paid_tax_incl = this.ps_total_paid_tax_incl.Replace(".", ",");
                this.wartoscBrutto = Convert.ToDecimal(this.ps_total_paid_tax_incl);
              }
              else if (xmlReader.Name == nameof (number))
              {
                xmlReader.Read();
                this.number = int.Parse(xmlReader.Value);
              }
              else if (xmlReader.Name == "id_order")
              {
                xmlReader.Read();
                this.idOrder = int.Parse(xmlReader.Value);
              }
            }
          }
        }
      }
    }
  }
}
