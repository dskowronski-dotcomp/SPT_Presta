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
        public const string ADRES_SKLEPU = "poundoutgear.com";
        public const string URLorders = "http://" + ADRES_SKLEPU +"/api/orders/";
        public const string URLcustomer = "http://" + ADRES_SKLEPU + "/api/customers/";
        //public const string URLcarrier = "http://" + ADRES_SKLEPU + "/presta/api/carriers/";
        public const string URLinvoices = "http://" + ADRES_SKLEPU + "/api/order_invoices/";
        public const string ps_login = "AF57LPXDD3TPVGYLBRRKUBFDWB58ZR17";

        /// <summary>
        /// TODO: Pobrać nazwę firmy, login operatora i hasło, następnie wrzucić zmienne do session - Login
        /// Ewentualnie znaleźć inne rozwiązanie tego problemu
        /// </summary>

        string companyName; 
        string user;
        string pass;

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
          for (int i = 24800; i <= 1000000; ++i)
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
              Customer customer = null;
                        try
                        {
                            customer = new Customer(new Zamowienie(faktura.IdOrder).IdCustomer);
                        } catch (Exception e)
                        {

                        }

              string idFakturyPresta = Convert.ToString(idFaktury);
                            ///string numerFakturyPresta = faktura.Number.ToString(this.fmt);

                            string numerFakturyPresta = idFaktury.ToString(this.fmt);

              SprzedazEwidencja sprzedazEwidencja = new SprzedazEwidencja();
              corem.DokEwidencja.AddRow((Row) sprzedazEwidencja);
              DefinicjaDokumentu definicjaDokumentu = corem.DefDokumentow.WgSymbolu["SPT"];

              sprzedazEwidencja.Definicja = definicjaDokumentu;
              sprzedazEwidencja.DataDokumentu = faktura.DataFaktury;
              sprzedazEwidencja.DataEwidencji = today;
              sprzedazEwidencja.DataWplywu = today;
              sprzedazEwidencja.NumerDokumentu = "#FV" + numerFakturyPresta + "/" + Convert.ToString(faktura.DataFaktury.Year);
              sprzedazEwidencja.Wartosc = (Soneta.Types.Currency) faktura.WartoscBrutto;
              sprzedazEwidencja.NumerDodatkowy = idFakturyPresta;

                            //sprzedazEwidencja.DomyślnaKasa.SposobZaplaty.Typ = Soneta.Kasa.TypySposobowZaplaty.Przelew;

              string kod = "!INCYDENTALNY";
              if(customer != null)
                  if (customer.Imie != null || customer.Nazwisko != null)
                    {
                        kod = customer.Imie + " " + customer.Nazwisko;
                    }

              if (kod.Length >= 19)
                kod = kod.Remove(17);
              Kontrahent kontrahent = cm.Kontrahenci.WgKodu[kod];
              Kontrahent kontrahentFirma = (Kontrahent) null;
              if (customer != null)
              if (customer.Nip != "" && customer.Nip != null)
                kontrahentFirma = cm.Kontrahenci.WgNIP[customer.Nip].FirstOrDefault<Kontrahent>();
              if (kontrahentFirma == null)
              {
                if (kontrahent == null)
                {
                  if (customer.Nip != "")
                  {
                    Kontrahent kontrahentNowy = new Kontrahent();
                    cm.Kontrahenci.AddRow((Row) kontrahentNowy);
                    //string str3 = kod.Length >= 17 ? customer.Company : customer.Company.Remove(17);
                    if (kod.Length >= 19)
                        kod = kod.Remove(17);
                    kontrahentNowy.Kod = kod;
                    kontrahentNowy.Nazwa = customer.Company;
                    kontrahentNowy.NIP = customer.Nip;
                    sprzedazEwidencja.Podmiot = (IPodmiot) kontrahentNowy;
                  }
                  else
                  {
                    Kontrahent kontrahentNowy = new Kontrahent();
                    cm.Kontrahenci.AddRow((Row) kontrahentNowy);
                    if (kod.Length >= 19)
                      kod = kod.Remove(17);
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
            string[] ps_order_id = new string[1000000];
            int[] ps_id = new int[1000000];

      WebRequest webRequest = WebRequest.Create(URLorders);
      webRequest.Credentials = (ICredentials) new NetworkCredential(ps_login, "");
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
                    ps_order_id[i] = xmlReader.Value;
                    ps_id[i] = int.Parse(ps_order_id[i]);
                }
              }
              xmlReader.MoveToElement();
            }
          }
        }
      }
      return Convert.ToString(((IEnumerable<int>) ps_id).Max());
    }

    public string IleFaktur()
    {

            int[] ps_id2 = new int[1000000];
            string[] ps_invoice_id = new string[1000000];
      
      int index1 = 0;
      WebRequest webRequest = WebRequest.Create(URLinvoices);
      webRequest.Credentials = (ICredentials) new NetworkCredential(ps_login, "");
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
                  ps_invoice_id[index1] = xmlReader.Value;
                  ps_id2[index1] = int.Parse(ps_invoice_id[index1]);
                  ++index1;
                }
              }
              xmlReader.MoveToElement();
            }
          }
        }
      }
      return Convert.ToString(((IEnumerable<int>) ps_id2).Max());
    }
  }
}
