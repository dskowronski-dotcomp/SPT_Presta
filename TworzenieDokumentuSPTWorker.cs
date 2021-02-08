// Decompiled with JetBrains decompiler
// Type: SPT_Presta.TworzenieDokumentuSPTWorker
// Assembly: spt_presta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3ED6FB4A-3869-4961-B24F-AC70524369F0
// Assembly location: C:\Program Files (x86)\Common Files\Soneta\Assemblies\spt_presta.dll

using Soneta.Business;
using Soneta.Business.App;
using Soneta.Core;
using Soneta.CRM;
using Soneta.EwidencjaVat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;

namespace SPT_Presta
{
  public class TworzenieDokumentuSPTWorker
  {
    public static string ps_id_address_delivery;
    public static string ps_id_carrier;
    public static string ps_customer_firstname;
    public static string ps_customer_lastname;
    public static int id_address_delivery;
    public static int id_customer;
    public static int id_carrier;
    public static string[] ps_productId = new string[40];
    public static string[] ps_product_quantity = new string[40];
    public static string[] ps_order_id = new string[100000];
    public static string[] ps_invoice_id = new string[100000];
    public static int[] product_id = new int[40];
    public static int[] quantity = new int[40];
    public static int[] product_quantity = new int[40];
    public const string URLorders = "http://localhost/presta/api/orders/";
    public const string URLcustomer = "http://localhost/presta/api/customers/";
    public const string URLcarrier = "http://localhost/presta/api/carriers/";
    public const string URLinvoices = "http://localhost/presta/api/order_invoices/";
    public const string ps_login = "XK96WGNV1WXAXYFRBYI2HTF7CMV3PZIB";
    public static int zmiennaProduktId = 0;
    public static int zmiennaIloscProduktu = 0;
    public static int[] ps_id = new int[100000];
    public static int[] ps_id2 = new int[100000];
    public string fmt = "000000.##";

    [Soneta.Business.Action("Tworzenie Dokumentu SPT", Mode = ActionMode.SingleSession | ActionMode.Progress, Target = ActionTarget.ToolbarWithText)]
    public void TworzenieDokumentu()
    {
      new Soneta.Start.Loader() { WithExtensions = true }.Load();
      using (Session session = BusApplication.Instance["Firma demo"].Login(false, "Administrator", "").CreateSession(false, false))
      {
        CRMModule cm = CRMModule.GetInstance((ISessionable) session);
        CoreModule corem = CoreModule.GetInstance((ISessionable) session);

        using (ITransaction transaction = session.Logout(true))
        {
          Soneta.Types.Date today = Soneta.Types.Date.Today;
          int num1 = 1;
          for (int i = 1; i <= 10000; ++i)
          {
            string numerdodatkowy = Convert.ToString(i);
            if (corem.DokEwidencja.WgDodatkowego[numerdodatkowy].IsEmpty)
            {
              num1 = i;
              break;
            }
          }

          int idOstatniejFaktury = int.Parse(this.IleFaktur());
          int idFaktury = num1;

          for (int index = num1; index <= idOstatniejFaktury; ++index)
          {
            string numerdodatkowy = Convert.ToString(index);

            if (corem.DokEwidencja.WgDodatkowego[numerdodatkowy].IsEmpty)
            {
              Faktura faktura = new Faktura(idFaktury);
              Customer customer = new Customer(new Zamowienie(faktura.idOrder).idCustomer);

              string idFakturyPresta = Convert.ToString(idFaktury);
              string numerFakuryPresta = faktura.number.ToString(this.fmt);

              SprzedazEwidencja sprzedazEwidencja = new SprzedazEwidencja();
              corem.DokEwidencja.AddRow((Row) sprzedazEwidencja);
              DefinicjaDokumentu definicjaDokumentu = corem.DefDokumentow.WgSymbolu["SPT"];

              sprzedazEwidencja.Definicja = definicjaDokumentu;
              sprzedazEwidencja.DataDokumentu = faktura.dataFaktury;
              sprzedazEwidencja.DataEwidencji = today;
              sprzedazEwidencja.DataWplywu = today;
              sprzedazEwidencja.NumerDokumentu = "#FV" + numerFakuryPresta + "/" + Convert.ToString(faktura.dataFaktury.Year);
              sprzedazEwidencja.Wartosc = (Soneta.Types.Currency) faktura.wartoscBrutto;
              sprzedazEwidencja.NumerDodatkowy = idFakturyPresta;

              string kod = customer.imie + " " + customer.nazwisko;

              Kontrahent kontrahent = cm.Kontrahenci.WgKodu[kod];
              Kontrahent kontrahentFirma = (Kontrahent) null;

              if (customer.nip != "")
                kontrahentFirma = cm.Kontrahenci.WgNIP[customer.nip].FirstOrDefault<Kontrahent>();
              if (kontrahentFirma == null)
              {
                if (kontrahent == null)
                {
                  if (customer.nip != "")
                  {
                    Kontrahent kontrahentNowy = new Kontrahent();
                    cm.Kontrahenci.AddRow((Row) kontrahentNowy);
                    string str3 = kod.Length < 18 ? customer.company : customer.company.Remove(18);
                    kontrahentNowy.Kod = str3;
                    kontrahentNowy.Nazwa = customer.company;
                    kontrahentNowy.NIP = customer.nip;
                    sprzedazEwidencja.Podmiot = (IPodmiot) kontrahentNowy;
                  }
                  else
                  {
                    Kontrahent kontrahentNowy = new Kontrahent();
                    cm.Kontrahenci.AddRow((Row) kontrahentNowy);
                    if (kod.Length >= 18)
                      kod.Remove(18);
                    kontrahentNowy.Kod = kod;
                    kontrahentNowy.Nazwa = kod;
                    sprzedazEwidencja.Podmiot = (IPodmiot) kontrahentNowy;
                  }
                }
                else
                  sprzedazEwidencja.Podmiot = (IPodmiot) kontrahent;
              }
              else
                sprzedazEwidencja.Podmiot = (IPodmiot) kontrahentFirma;
              sprzedazEwidencja.Stan = StanEwidencji.Bufor;

              transaction.CommitUI();
              ++num1;
              ++idFaktury;
            }
          }
        }
        session.Save();
      }
    }

    public string IleZamowien()
    {
      WebRequest webRequest = WebRequest.Create("http://localhost/presta/api/orders/");
      webRequest.Credentials = (ICredentials) new NetworkCredential("XK96WGNV1WXAXYFRBYI2HTF7CMV3PZIB", "");
      using (WebResponse response = webRequest.GetResponse())
      {
        using (XmlReader xmlReader = XmlReader.Create(response.GetResponseStream()))
        {
          while (xmlReader.Read())
          {
            if (xmlReader.HasAttributes)
            {
              for (int i = 0; i < xmlReader.AttributeCount; ++i)
              {
                xmlReader.MoveToAttribute(i);
                if (xmlReader.Name == "id")
                {
                  TworzenieDokumentuSPTWorker.ps_order_id[i] = xmlReader.Value;
                  TworzenieDokumentuSPTWorker.ps_id[i] = int.Parse(TworzenieDokumentuSPTWorker.ps_order_id[i]);
                }
              }
              xmlReader.MoveToElement();
            }
          }
        }
      }
      return Convert.ToString(((IEnumerable<int>) TworzenieDokumentuSPTWorker.ps_id).Max());
    }

    public string IleFaktur()
    {
      int index1 = 0;
      WebRequest webRequest = WebRequest.Create("http://localhost/presta/api/order_invoices/");
      webRequest.Credentials = (ICredentials) new NetworkCredential("XK96WGNV1WXAXYFRBYI2HTF7CMV3PZIB", "");
      using (WebResponse response = webRequest.GetResponse())
      {
        using (XmlReader xmlReader = XmlReader.Create(response.GetResponseStream()))
        {
          while (xmlReader.Read())
          {
            if (xmlReader.HasAttributes)
            {
              for (int index2 = 0; index2 < xmlReader.AttributeCount; ++index2)
              {
                xmlReader.MoveToAttribute(0);
                if (xmlReader.Name == "id")
                {
                  TworzenieDokumentuSPTWorker.ps_invoice_id[index1] = xmlReader.Value;
                  TworzenieDokumentuSPTWorker.ps_id2[index1] = int.Parse(TworzenieDokumentuSPTWorker.ps_invoice_id[index1]);
                  ++index1;
                }
              }
              xmlReader.MoveToElement();
            }
          }
        }
      }
      return Convert.ToString(((IEnumerable<int>) TworzenieDokumentuSPTWorker.ps_id2).Max());
    }
  }
}
