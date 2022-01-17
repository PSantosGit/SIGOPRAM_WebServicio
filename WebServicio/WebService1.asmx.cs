using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;


namespace WebServicio
{
   /// <summary>
   /// Summary description for WebService1
   /// </summary>
   [WebService(Namespace = "http://tempuri.org/")]
   [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
   [System.ComponentModel.ToolboxItem(false)]
   // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
   // [System.Web.Script.Services.ScriptService]
   public class WebService1 : System.Web.Services.WebService
   {


      [WebMethod]
      public void CalculaDimesionProblemaPL(String sInputlArcos, String sInputlNodos, String user, String macAdress, String dominio, String pw, String version)
      {

         var lPLArcos = JsonConvert.DeserializeObject<List<cWSArcos>>(sInputlArcos);
         var lPLNodos = JsonConvert.DeserializeObject<List<cWSNodos>>(sInputlNodos);
         var respWS = new RespuestaWS<List<int>>();
         var sigopramWS = new Sigopram_WS();
         int LicenseStatus;
         LicenseStatus = sigopramWS.CheckLicense(user, macAdress, version, "pc", dominio, pw, FlagLicencelog.ControlLicNOWrite);
         if (LicenseStatus == 1)
         {
            try
            {
               respWS.serviceResult = sigopramWS.CalculaDimesionProblemaPL(lPLArcos, lPLNodos);
               respWS.responseStatus = 0;
               respWS.returnMessage = "Credenciales y cálculo correctos.";
            }
            catch (Exception ex)
            {
               respWS.returnMessage = "Credenciales correctas. Error en el proceso de cálculo." + ex.Message;
               respWS.responseStatus = -1;
               respWS.serviceResult = null;
            }
         }
         else
         {
            respWS.returnMessage = "Credenciales incorrectas. Proceso de optimización interrumpido!";
            respWS.responseStatus = -2;
            respWS.serviceResult = null;
         }

         Context.Response.ContentType = "application/json";
         Context.Response.Write(JsonConvert.SerializeObject(respWS));
      }

      [WebMethod]
      public void GetEnetPart1(String sInputlArcos, String sInputlNodos, String sCarpetaProyecto, String user, String macAdress, String version, String pcName, String dominio, String pw)
      {
         var lPLArcos = JsonConvert.DeserializeObject<List<cWSArcos>>(sInputlArcos);
         var lPLNodos = JsonConvert.DeserializeObject<List<cWSNodos>>(sInputlNodos);
         var respWS = new RespuestaWS<string>();
         var sigopramWS = new Sigopram_WS();
         int LicenseStatus;
         LicenseStatus = sigopramWS.CheckLicense(user, macAdress, version, pcName, dominio, pw, FlagLicencelog.ControlLicAndWrite);
         if (LicenseStatus==1)
         {
            try
            {
               respWS.responseStatus = 0;
               respWS.serviceResult = sigopramWS.GetEnetHeader(lPLNodos, lPLArcos, sCarpetaProyecto);
               respWS.returnMessage = "Credenciales y cálculo correctos.";
            }
            catch (Exception ex)
            {
               respWS.responseStatus = -1;
               respWS.serviceResult = null;
               respWS.returnMessage = "Credenciales correctas. Error en el proceso de cálculo." + ex.Message;
               
            }
         }
         else
         {
            respWS.returnMessage = "Credenciales incorrectas. Proceso Interrumpido!";
            respWS.responseStatus = -2;
            respWS.serviceResult = null;
         }

         Context.Response.ContentType = "application/json";
         Context.Response.Write(JsonConvert.SerializeObject(respWS));
      }

      [WebMethod]
      public void ControlLicence(String user, String macAdress, String version, String pcName, String dominio, String pw, Boolean bWriteLog)
      {
         var respWS = new RespuestaWS<int>();
         var sigopramWS = new Sigopram_WS();

         try
         {
            if (bWriteLog)
            {
               respWS.serviceResult = sigopramWS.CheckLicense(user, macAdress, version, pcName, dominio, pw, FlagLicencelog.ControlLicAndWrite);
            }
            else
            {
               respWS.serviceResult = sigopramWS.CheckLicense(user, macAdress, version, pcName, dominio, pw,FlagLicencelog.ControlLicNOWrite);
            }
            if (respWS.serviceResult == 1)
            { respWS.returnMessage = "Usuario: " + user + " con licencia activa!";}
            else
            { respWS.returnMessage = "Usuario: " + user + " sin licencia activa!"; }

            respWS.responseStatus = 0;
         }
         catch (Exception ex)
         {
            respWS.returnMessage = ex.Message;
            respWS.responseStatus = -1;
            respWS.serviceResult = -1;
         }
         Context.Response.ContentType = "application/json";
         Context.Response.Write(JsonConvert.SerializeObject(respWS));
      }

      [WebMethod]
      public void ForceLicence(String user, String macAdress, String version, String pcName, String dominio, String pw, String forcedIdSerial)
      {
         var respWS = new RespuestaWS<int>();
         var sigopramWS = new Sigopram_WS();

         try
         {
               respWS.serviceResult = sigopramWS.ForceLicense(user, macAdress, version, pcName, dominio, pw, forcedIdSerial);
  
            if (respWS.serviceResult == 1)
            { respWS.returnMessage = "Usuario: " + user + " asignado a MacAdress: " + macAdress; }
            else
            { respWS.returnMessage = "Nose ha podido asignar macAdress " +macAdress + " al usuario " + user; }

            respWS.responseStatus = 0;
         }
         catch (Exception ex)
         {
            respWS.returnMessage = ex.Message;
            respWS.responseStatus = -1;
            respWS.serviceResult = -1;
         }
         Context.Response.ContentType = "application/json";
         Context.Response.Write(JsonConvert.SerializeObject(respWS));
      }
      [WebMethod]
      public void returnSerials(String user, String macAdress, String version, String dominio, String pw)
      {
         var respWS = new RespuestaWS<string>();
         var sigopramWS = new Sigopram_WS();
         try
         {
            respWS.serviceResult = sigopramWS.ReturnSerials(user, macAdress, version, dominio, pw);
            respWS.returnMessage = "serials obtenidos correctamente!"; 
            respWS.responseStatus = 0;
         }
         catch (Exception ex)
         {
            respWS.returnMessage = ex.Message;
            respWS.responseStatus = -1;
            respWS.serviceResult = "";
         }
         Context.Response.ContentType = "application/json";
         Context.Response.Write(JsonConvert.SerializeObject(respWS));
      }

      [WebMethod]
      public void GetU(String user, String macAdress, String version, String pcName, String dominio, String pw)
      {
         var respWS = new RespuestaWS<double[]>();
         var sigopramWS = new Sigopram_WS();
         int LicenseStatus;
         LicenseStatus = sigopramWS.CheckLicense(user, macAdress, version, pcName, dominio, pw, FlagLicencelog.ControlLicNOWrite);
         if (LicenseStatus == 1)
         {
            try
            {
               respWS.responseStatus = 0;
               respWS.serviceResult = sigopramWS.GetU();
               respWS.returnMessage = "Credenciales y cálculo de U correctos.";
            }
            catch (Exception ex)
            {
               respWS.responseStatus = -1;
               respWS.serviceResult = null;
               respWS.returnMessage = "Credenciales correctas. Error en el proceso de cálculo U." + ex.Message;

            }
         }
         else
         {
            respWS.returnMessage = "Credenciales incorrectas. Proceso Interrumpido!";
            respWS.responseStatus = -2;
            respWS.serviceResult = null;
         }

         Context.Response.ContentType = "application/json";
         Context.Response.Write(JsonConvert.SerializeObject(respWS));
      }

   }
   public class RespuestaWS<T>
   {
      public T serviceResult;
      public int responseStatus;
      public string returnMessage;
      public RespuestaWS() { }
         }

}
