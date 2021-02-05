using ExchangeRatesMcr.CbrConnectorApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ExchangeRatesMcr.CbrConnectorApp
{
    public class ConnectionCbr
    {
        private HttpClient client;
        const string urlValuteModel = "http://www.cbr.ru/scripts/XML_val.asp?d=0";
        const string urlValueNominals = "http://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={0}&date_req2={1}&VAL_NM_RQ={2}";
        public ConnectionCbr()
        {
            client = CreateHttpClient();
        }

        /// <summary>
        /// Создать экземпляр HttpClient
        /// </summary>
        /// <returns></returns>
        private HttpClient CreateHttpClient()
        {            
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
            httpClientHandler.AllowAutoRedirect = true;

            // создаем объект клиента HTTP
            return new HttpClient(handler: httpClientHandler, disposeHandler: true);
        }

        /// <summary>
        /// Получить список валют
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ValutaModel>> GetValuteModel()
        {
            List<ValutaModel> valutaModels = new List<ValutaModel>();
            var response = await client.GetAsync(urlValuteModel);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var ss = await response.Content.ReadAsStringAsync();
                //TODO: Переделать на нормальную сериализацию XML
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(ss);
                // получим корневой элемент
                XmlElement xRoot = xDoc.DocumentElement;
                // обход всех узлов в корневом элементе
                foreach (XmlNode xnode in xRoot)
                {
                    ValutaModel newModel = new ValutaModel();
                    // обходим все дочерние узлы элемента 
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        switch (childnode.Name)
                        {
                            case "Name":
                                newModel.Name = childnode.InnerText.Trim();
                                break;
                            case "EngName":
                                newModel.EngName = childnode.InnerText.Trim();
                                break;
                            case "Nominal":
                                newModel.Nominal = double.Parse(childnode.InnerText.Trim());
                                break;
                            case "ParentCode":
                                newModel.ParentCode = childnode.InnerText.Trim();
                                break;
                        }
                    }
                    valutaModels.Add(newModel);
                }

                return valutaModels;
            }

            throw new ArgumentNullException();
        }

        /// <summary>
        /// Получить список исторический список значений указаной валюты
        /// </summary>
        /// <param name="dateFrom">дата c</param>
        /// <param name="dateTo">дата по</param>
        /// <param name="valueId">код валюты</param>
        /// <returns></returns>
        public async Task<IEnumerable<ValutaNominal>> GetValuteNominals(DateTime dateFrom, DateTime dateTo, string valueId)
        {
            List<ValutaNominal> valutaNominalsModels = new List<ValutaNominal>();
            var typeUrl = string.Format(urlValueNominals, dateFrom.ToString("dd/MM/yyyy"), dateTo.ToString("dd/MM/yyyy"), valueId);
            var response = await client.GetAsync(string.Format(urlValueNominals, dateFrom.ToString("dd/MM/yyyy"), dateTo.ToString("dd/MM/yyyy"), valueId));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var ss = await response.Content.ReadAsStringAsync();
                //TODO: Переделать на нормальную сериализацию XML
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(ss);
                // получим корневой элемент
                XmlElement xRoot = xDoc.DocumentElement;
                // обход всех узлов в корневом элементе
                foreach (XmlNode xnode in xRoot)
                {
                    ValutaNominal newModel = new ValutaNominal();
                    //обходим все аттрибуты
                    foreach (XmlAttribute attribute in xnode.Attributes)
                    {
                        if (attribute.Name == "Date")
                            newModel.RecordDate = DateTime.Parse(attribute.Value);
                        if (attribute.Name == "Id")
                            newModel.ValuteId = attribute.Value;
                    }

                    // обходим все дочерние узлы элемента 
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        switch (childnode.Name)
                        {
                            case "Nominal":
                                newModel.Nominal = double.Parse(childnode.InnerText.Trim());
                                break;
                            case "Value":
                                newModel.Value = double.Parse(childnode.InnerText.Trim());
                                break;
                        }
                    }
                    valutaNominalsModels.Add(newModel);
                }

                return valutaNominalsModels;
            }

            throw new ArgumentNullException();
        }
    }
}
