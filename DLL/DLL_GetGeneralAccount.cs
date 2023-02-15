using System.Net;
using System.Xml.Linq;
using System.Xml;
using System;
using System.Collections;
using static IMAL_Services.DLL_GetGeneralAccount;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;
using System.Security.Principal;
using System.Threading.Channels;

namespace IMAL_Services

{
    public class DLL_GetGeneralAccount
    {
        public static HttpWebRequest CreateWebRequest()
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var GeneralAccountUrl = MyConfig.GetValue<string>("AppSettings:GeneralAccountUrl");
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(GeneralAccountUrl);
            webRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        public static HttpWebRequest CreateWebRequestGetCIF()
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var GeneralAccountUrl = MyConfig.GetValue<string>("AppSettings:GetCIFUrl");
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(GeneralAccountUrl);
            webRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        public static HttpWebRequest CreateWebRequestGetSpicContition()
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var GetSpicalCondition = MyConfig.GetValue<string>("AppSettings:GetSpicalCondition");
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(GetSpicalCondition);
            webRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        public ArrayList returnCIFDetails(string CIFno, string username, string password)
        {
            string StatusDesc = "";
            string StatusCode = "";
            string transactionTime = Convert.ToString(DateTime.Now);
            HttpWebRequest request = CreateWebRequestGetCIF();
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:cif=""cifManagementWs"">
   <soapenv:Header/>
   <soapenv:Body>
      <cif:returnCifDetails>
       
         <serviceContext>
           
            <businessArea>Retail</businessArea>
          
            <businessDomain>Products</businessDomain>
            
            <operationName>returnCifDetails</operationName>
            
            <serviceDomain>CifManagement</serviceDomain>
         
            <serviceID>1914</serviceID>
            
            <version>1.0</version>
         </serviceContext>
         <companyCode>1</companyCode>
         <branchCode>5599</branchCode>
         <cifNumber>"+ CIFno + @"</cifNumber>
       
<!--
         <requestContext>
            
            <requestID>?</requestID>
           
            <coreRequestTimeStamp>?</coreRequestTimeStamp>
        
            <requestKernelDetails>?</requestKernelDetails>
         </requestContext>
       -->
         <requesterContext>
            <langId>EN</langId>
            <password>"+ password + @"</password>
            <requesterTimeStamp>2021-11-10T09:00:00</requesterTimeStamp>
            <userID>"+ username +@"</userID>
         </requesterContext>
     
         <vendorContext>
            <!--You may enter the following 3 items in any order-->
            <!--Optional:-->
            <license>Copyright 2018 Path Solutions. All Rights Reserved</license>
            <!--Optional:-->
            <providerCompanyName>Path Solutions</providerCompanyName>
            <!--Optional:-->
            <providerID>IMAL</providerID>
         </vendorContext>
      </cif:returnCifDetails>
   </soapenv:Body>
</soapenv:Envelope>



");
            string soapResult = "";

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
            var type = "";
            var status = "";
            var kyc = "";
            var cifIsComplete = "";
            var longNameArabic = "";
            var longNameEnglish = "";
            var Error = "NA";
            var customerName = "";
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                   // Console.WriteLine(soapResult);
                    var str = XElement.Parse(soapResult);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(soapResult);
                    XmlNodeList elemlistCode = xmlDoc.GetElementsByTagName("statusCode");
                    StatusCode = elemlistCode[0].InnerXml;

                    if (StatusCode == "0")
                    {
                        XmlNodeList elemlist_type = xmlDoc.GetElementsByTagName("type");
                        type = elemlist_type[0].InnerXml;
                        XmlNodeList elemlist_status = xmlDoc.GetElementsByTagName("status");
                        status = elemlist_status[0].InnerXml;
                        XmlNodeList elemlist_kyc = xmlDoc.GetElementsByTagName("kyc");
                        kyc = elemlist_kyc[0].InnerXml;
                        XmlNodeList elemlist_cifIsComplete = xmlDoc.GetElementsByTagName("cifIsComplete");
                        cifIsComplete = elemlist_cifIsComplete[0].InnerXml;
                        XmlNodeList elemlist_longNameArabic = xmlDoc.GetElementsByTagName("longNameArabic");
                        longNameArabic = elemlist_longNameArabic[0].InnerXml;
                        XmlNodeList elemlist_longNameEnglish = xmlDoc.GetElementsByTagName("longNameEnglish");
                        longNameEnglish = elemlist_longNameArabic[0].InnerXml;


                        if (status != "A")
                        {

                            Error = "Account not Active";
                        }
       
                            if (longNameArabic.Length != 0)
                            {
                                customerName = longNameArabic;
                            }
                            else
                            {
                                if (longNameEnglish.Length != 0)
                                {
                                    customerName = longNameEnglish;
                                }
                            }
                   
            
                       
                    }
                    else
                    {
                        XmlNodeList elemlistDesc = xmlDoc.GetElementsByTagName("statusDesc");
                        StatusDesc = elemlistDesc[0].InnerXml;
                        Error = StatusDesc;
                    }
                }



            }
            ArrayList arrayList = new ArrayList();
            arrayList.Add(Error);
            arrayList.Add(customerName);
            arrayList.Add(type);
        
            return arrayList;
        }

        public ArrayList ReturnGeneralAccount(string accountref,string username,string password)
        {
            
            string transactionTime = Convert.ToString(DateTime.Now);
            var acc = accountref;
            HttpWebRequest request = CreateWebRequest();
            XmlDocument soapEnvelopeXml = new XmlDocument();
           


            soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:gen=""generalAccountsWs"">
   <soapenv:Header/>
   <soapenv:Body>
      <gen:returnGeneralAccountDetails>
         <!--You may enter the following 10 items in any order-->
         <!--Optional:-->
         <serviceContext>
            <!--You may enter the following 6 items in any order-->
            <!--Optional:-->
            <businessArea>Retail</businessArea>
            <!--Optional:-->
            <businessDomain>Products</businessDomain>
            <!--Optional:-->
            <operationName>returnGeneralAccountDetails</operationName>
            <!--Optional:-->
            <serviceDomain>GeneralAccounts</serviceDomain>
            <!--Optional:-->
            <serviceID>801</serviceID>
            <!--Optional:-->
            <version>1.0</version>
         </serviceContext>

         
         <companyCode>1</companyCode>
 <!-- Default Value = 1 -->
         <branchCode>5599</branchCode>
 <!-- Default Value = 5599 -->
         

         <account>
         
            <!-- <branch>?</branch>
            <currency>?</currency>
            <accGl>?</accGl>
            <serialNo>?</serialNo>
            <cif>?</cif>

            
            <ibanAccNo>?</ibanAccNo>
 -->
            <additionalRef>" + accountref + @"</additionalRef>
 <!-- Parameter -->
         </account>

         
          <showAccountHistoryDetails>0</showAccountHistoryDetails>
 <!-- Default Value = 0 -->
         <showAdditionalFields>0</showAdditionalFields>
 <!-- Default Value = 0 -->
         <showChargesDetails>0</showChargesDetails>
 <!-- Default Value = 0 -->
         
          <requestContext>
            <!--<requestID>?</requestID>-->
            <!--<coreRequestTimeStamp>?</coreRequestTimeStamp>-->
            <!--<requestKernelDetails>?</requestKernelDetails>-->
         </requestContext>
         <requesterContext>
            <channelID>1</channelID> <!-- Default Value = 1 -->
            <hashKey>1</hashKey> <!-- Default Value = 1 -->
              <langId>EN</langId> <!-- Default Value = EN -->
            <password>"+ password + @"</password> <!-- Parameter -->
            <requesterTimeStamp>2022-01-03T09:00:00 </requesterTimeStamp> <!-- Parameter -->
            <userID>"+ username +@"</userID> <!-- Parameter -->
         </requesterContext>

         <!--Optional:-->
         <vendorContext>
            <!--You may enter the following 3 items in any order-->
            <!--Optional:-->
            <license>Copyright 2018 Path Solutions. All Rights Reserved</license>
            <!--Optional:-->
            <providerCompanyName>Path Solutions</providerCompanyName>
            <!--Optional:-->
            <providerID>IMAL</providerID>
         </vendorContext>
      </gen:returnGeneralAccountDetails>
   </soapenv:Body>
</soapenv:Envelope>
");
            string soapResult = "";

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
            string currencyDescription = "";
            var status = "";
            var CifNo = "";
            var briefNameArabic = "";
            var BranchName = "";
            string Error = "";
            string statusCode = "";
            string statusDesc = "";
            string branchCode = "";
        
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                    //Console.WriteLine(soapResult);
                    var str = XElement.Parse(soapResult);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(soapResult);
                    XmlNodeList elemlistCode = xmlDoc.GetElementsByTagName("statusCode");
                    statusCode = elemlistCode[0].InnerXml;

                    if (statusCode == "0")
                    {

                        XmlNodeList elemlist = xmlDoc.GetElementsByTagName("status");
                        status = elemlist[0].InnerXml;
                        XmlNodeList elemlist1 = xmlDoc.GetElementsByTagName("cifNo");
                        CifNo = elemlist1[0].InnerXml;

                        XmlNodeList elemlist2 = xmlDoc.GetElementsByTagName("currencyDescription");
                        currencyDescription = elemlist2[0].InnerXml;


                        XmlNodeList elemlist4 = xmlDoc.GetElementsByTagName("branch");
                        BranchName = elemlist4[0].InnerXml;
                        XmlNodeList elemlist5 = xmlDoc.GetElementsByTagName("branchCode");
                        branchCode = elemlist5[0].InnerXml;
                        if (status == "Active")
                        {
                            Error = "NA";
                        }
                        else
                        {
                            Error = status;
                        }
                    }
                    else
                    {
                        XmlNodeList elemlistDesc = xmlDoc.GetElementsByTagName("statusDesc");
                        statusDesc = elemlistDesc[0].InnerXml;
                        Error = statusDesc;
                    }
                }
            }


            ArrayList arrayList= new ArrayList();
            arrayList.Add(Error);
            arrayList.Add(CifNo);
            arrayList.Add(currencyDescription);
            arrayList.Add(BranchName);
            arrayList.Add(branchCode);
            return arrayList;
        }

        public string GetaccountDetails(string accountref, string username, string password)
        {
            string CIFNo = "";
            string Error_GeneralAccount = "";
            string Error_CIF = "";
            string currencyDescription = "";
            string branch = "";
            string CustomerName = "";
            string Type = "";
            string Error = "";
            string branchCode = "";
            string specilcondition = "";
            string StatusDesc = "";
            List<Datum> datum = new List<Datum>();
            //try
            //{

                ArrayList ResultGeneralAccount = ReturnGeneralAccount(accountref, username, password);

      
                    Error_GeneralAccount = ResultGeneralAccount[0].ToString();
                    CIFNo = ResultGeneralAccount[1].ToString();
                    currencyDescription = ResultGeneralAccount[2].ToString();
                    branch = ResultGeneralAccount[3].ToString();
                    branchCode = ResultGeneralAccount[4].ToString();


            if (Error_GeneralAccount == "NA")
                {
                   ArrayList ResultCIfDetails = returnCIFDetails(CIFNo, username, password);

          
                    CustomerName = ResultCIfDetails[1].ToString();
                    Type = ResultCIfDetails[2].ToString();
                if(Type =="V")
                {
                    Type = "Retail";
                }
                else
                {
                    if (Type == "T")
                    {
                        Type = "Corporate";
                            }
                }
                    Error_CIF = ResultCIfDetails[0].ToString();


                    
                    if (Error_CIF != "NA")

                    {
                        Error = Error_CIF;
                    }
                    else
                {
                    ArrayList ReturnSpecialCOdtion = ReturnSpicContition(accountref,username,password, branch);
                    specilcondition = ReturnSpecialCOdtion[0].ToString();
                 StatusDesc   = ReturnSpecialCOdtion[1].ToString();
                    if (StatusDesc != "Success")
                    {
                        if(StatusDesc == "No records found.")
                        {
                            Error = "NA";
                        }
                    }
                    else
                    {
                        if (specilcondition == "Forbid Both")
                        {
                            Error = specilcondition;
                        }
                        else
                        {
                            if (specilcondition == "Forbid Credit")
                            {
                                Error = specilcondition;

                            }
                          

                        }
                    }
                    
                }
                  
            }
                else
                {
                    Error = Error_GeneralAccount;
                }
      
                if(Error !="NA")
            {
                CustomerName = "";
                CIFNo = "";
                Type = "";
                currencyDescription = "";
                branch = "";
            }
                
                datum.Add(new Datum
            {
                CustomerName = CustomerName,
                CustomerCIf = CIFNo,
                CustomerType = Type,
                AccountCurrancy = currencyDescription,
                Branch = branch,
                Response_Error = Error

            });
            return JsonConvert.SerializeObject(datum);
        }

        public ArrayList ReturnSpicContition(string additionalRef, string username, string password,string BranchCode )
        {
            string StatusCode = "";
         

            string transactionTime = Convert.ToString(DateTime.Now);
       
            HttpWebRequest request = CreateWebRequestGetSpicContition();
            XmlDocument soapEnvelopeXml = new XmlDocument();



            soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:gen=""specialConditionWs"">
    <soapenv:Header/>
   <soapenv:Body>
      <gen:returnSpecialConditionList>
         <!--You may enter the following 23 items in any order-->
         <!--Optional:-->
         <serviceContext>
            <!--You may enter the following 6 items in any order-->
            <!--Optional:-->
            <businessArea>Retail</businessArea>
            <!--Optional:-->
            <businessDomain>Products</businessDomain>
            <!--Optional:-->
            <operationName>returnSpecialConditionList</operationName>
            <!--Optional:-->
            <serviceDomain>SpecialCondition</serviceDomain>
            <!--Optional:-->
            <serviceID>9703</serviceID>
            <!--Optional:-->
            <version>1.0</version>
         </serviceContext>
         <companyCode>1</companyCode>
         <branchCode>"+BranchCode+@"</branchCode>
         <!--Optional:-->
<!--         <lineNo>?</lineNo>
-->
         <!--Optional:-->
<!--         <entityType>?</entityType>
-->
         <!--Optional:-->
<!--         <cifNo>333270</cifNo>
-->
         <!--Optional:-->
         
          <account>
<!--            <branch>1024</branch>-->
<!--            <currency>818</currency>-->
<!--            <accGl>211106</accGl>-->
<!--            <serialNo>0</serialNo>-->
<!--            <cif>333270</cif>-->

            <additionalRef>"+additionalRef+@"</additionalRef>
<!--            <ibanAccNo>?</ibanAccNo>
-->
         </account>
 

         
         <!-- <forbidTrx>?</forbidTrx>
         <forbidProduct>?</forbidProduct>
         <reasonEng>?</reasonEng>
         <reasonArab>?</reasonArab>
         <startingDate>?</startingDate>
         <expiryDate>?</expiryDate>
         <recordSource>?</recordSource>
         <dateJudgement>?</dateJudgement>
         <status>?</status>
         <description>?</description>
 -->
         
         
         <!-- <sortOrder>
            <sord>?</sord>
            <sidx>?</sidx>
         </sortOrder>
 -->

         
         <!-- <pagination>
            <pageSize>?</pageSize>
            <pageNumber>?</pageNumber>
            <totalCount>?</totalCount>
         </pagination>
 -->

         
         <!-- <dynamicFilter>
            <allAny>All</allAny>
            <filters>
               <filter>
                  <key>?</key>
                  <operator>?</operator>
                  <value>?</value>
               </filter>
            </filters>
         </dynamicFilter>
 -->
         


         
         <requestContext>
            <!--<requestID>?</requestID>-->
            <!--<coreRequestTimeStamp>?</coreRequestTimeStamp>-->
            <!--<requestKernelDetails>?</requestKernelDetails>-->
         </requestContext>

         
         <requesterContext>
            <channelID>1</channelID>
            <hashKey>1</hashKey>
              <langId>EN</langId>
            <password>"+password+@"</password>
            <requesterTimeStamp>2023-01-03T09:00:00</requesterTimeStamp>
            <userID>"+username+@"</userID>
         </requesterContext>

         

         <!--Optional:-->
         <vendorContext>
            <!--You may enter the following 3 items in any order-->
            <!--Optional:-->
            <license>Copyright 2018 Path Solutions. All Rights Reserved</license>
            <!--Optional:-->
            <providerCompanyName>Path Solutions</providerCompanyName>
            <!--Optional:-->
            <providerID>IMAL</providerID>
         </vendorContext>
      </gen:returnSpecialConditionList>
   </soapenv:Body>
</soapenv:Envelope>
");
            string soapResult = "";

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
            string forbidTransactionDescription = "";
            string Error = "";
            string statusDesc = "";

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                    Console.WriteLine(soapResult);
                    var str = XElement.Parse(soapResult);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(soapResult);
                    XmlNodeList elemlistCode = xmlDoc.GetElementsByTagName("statusCode");
                    StatusCode = elemlistCode[0].InnerXml;

                    if (StatusCode == "0")
                    {
                        XmlNodeList spicalConditionlist = xmlDoc.GetElementsByTagName("forbidTransactionDescription");
                        forbidTransactionDescription = spicalConditionlist[0].InnerXml;
                        XmlNodeList elemlistDesc = xmlDoc.GetElementsByTagName("statusDesc");
                        statusDesc = elemlistDesc[0].InnerXml;

                    }
                    else
                    {
                        XmlNodeList elemlistDesc = xmlDoc.GetElementsByTagName("statusDesc");
                        statusDesc = elemlistDesc[0].InnerXml;
                     
                    }
                }
            }

            ArrayList arrayList = new ArrayList();
            arrayList.Add(forbidTransactionDescription);
            arrayList.Add(statusDesc);
            return arrayList;
        }
        public class Datum
        {
            public string CustomerName { get; set; }
            public string CustomerCIf { get; set; }

            public string AccountCurrancy { get; set; }
            public string CustomerType { get; set; }
            public string Response_Error { get; set; }
            public string Branch { get; set; }

        }
    }
}
