using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WebServicio.es.sigopram;

namespace WebServicio
{
    public  enum FlagLicencelog
   {
      ControlLicNOWrite = 0,
      ForceLicOnPCName = 1,
      ControlLicAndWrite = 2,
      LibertaPW = 3,
      SetNewPassWord = 4
   }
   public class Sigopram_WS
   {
          public List<int> CalculaDimesionProblemaPL(List<cWSArcos> lPLArcos, List<cWSNodos> lPLNodos)
      {

         // Calcula el numero de restricciones  hidraulicos del problema
         var NumRecorridos = lPLNodos.Count;
         var NumRestPmin = lPLNodos.FindAll(w => w.sysTag == 1 & (w.Tipus == "GOT" | w.Tipus == "TOM" | w.Tipus == "EMI" | w.Tipus == "RED" | w.Tipus == "CRC")).Count;
         var NumRestPmax = lPLNodos.FindAll(w => w.sysTag == 1 & (w.Tipus == "EMI" | w.Tipus == "GOT")).Count;
         var NRestricionesHid = NumRestPmin + NumRestPmax;

         var PLJMatColCount = lPLArcos.Sum(w => w.NTubosCandidatos);
         int NLin, NCol;
         NLin = NRestricionesHid;
         NCol = PLJMatColCount;
         List<int> lResults = new List<int>();
         lResults.Add(NLin);
         lResults.Add(NCol);
         return lResults;
      }


      public string GetResID(List<cWSNodos> lPLNodos)
      {
         var IDRes = lPLNodos.Find(w => w.Tipus == "RES").IDNode;
         return IDRes;
      }

      public string GetEnetHeader(List<cWSNodos> lPLNodos, List<cWSArcos> lPLArcos, string sCarpetaProyecto)
      {
         StringBuilder sb = new StringBuilder();
         var nNodos = lPLNodos.Count;
         var NArcos = lPLArcos.Count;
         var sTitle = sCarpetaProyecto + "Num Nodos=" + nNodos + "  Num Arcos=" + NArcos;
         sb.AppendLine("[TITLE]");
         sb.AppendLine(sTitle);
         sb.AppendLine("");
         sb.AppendLine("[JUNCTIONS]");
         sb.AppendLine(";ID                  Elev            Demand          Pattern         ");

         return sb.ToString();
      }


      public int CheckLicense(String user, String macAdress, String version, String pcName, String dominio, String pw, FlagLicencelog iFlag)
      {
         System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
         string strEnvia="";
         
         if (iFlag == FlagLicencelog.ForceLicOnPCName)
         { }

         if (iFlag == FlagLicencelog.LibertaPW)
         { }

         if (iFlag == FlagLicencelog.SetNewPassWord)
         { }

         if (iFlag == FlagLicencelog.ControlLicAndWrite | iFlag == FlagLicencelog.ControlLicNOWrite)
         {  strEnvia = user + ";" + macAdress + ";" + version + ";" + pcName + ";" + (int)iFlag + ";" + dominio; }

         GetClau gcWS = new GetClau();
         var resp =  gcWS.CallGetClau(strEnvia);

         //Ejemplo de registros de la tabla serials retornada
         //Cada registro está separado por "#" y cada campo por ";"
         //strRet = "4;PSANTOS;;B1EF0F30;2020-12-31 00:00:00;1;ASG;375;0;Un eventual mensaje en la BD;1;;ASG-PC02(eariño);-4;9999999;1;ABCD#2000;PSANTOS;;87AF8170;2020-12-31 00:00:00;1;ASG;375;0;Un eventual mensaje en la BD;1;;ASG-PC31(Administrador);0;9999999;1;ABCD"
         //posicio  0    1    2    3            4           5   6  7  8              9            1011   12                13    14   15 16   0      1    2     3            4          5  6   7  8              9              10 11   12                  13  14    15  16
         //var strarrayret1 ="";
               

            if (iFlag == FlagLicencelog.ControlLicAndWrite | iFlag == FlagLicencelog.ControlLicNOWrite)
            {
               var strarrayret1 = resp.Split(';');
               if (strarrayret1[5] == "1" & strarrayret1[1]==user & strarrayret1[3]==macAdress)
               {
                  return 1;
               }
               else
               {
                  return 0;
               }
            }
            else
            {
               return 0;
            }
         
         
      }

      public int ForceLicense(String user, String macAdress, String version, String pcName, String dominio, String pw, String forcedIdSerial)
      {
         System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
         string strEnvia = "";
         // Para forzar Licencia :  4 ; D4C40AFB ; 540i ; DESKTOP-Q4EUMOG(Pedro); 1 ; DESKTOP-Q4EUMOG
          strEnvia = forcedIdSerial + ";" + macAdress + ";" + version + ";" + pcName + ";" + "1" + ";" + pcName;
                
         GetClau gcWS = new GetClau();
         var resp = gcWS.CallGetClau(strEnvia);
         return 1;
      }

      public string ReturnSerials(String user, String macAdress, String version, String dominio, String pw)
      {
         System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
         string strEnvia = user + ";" + macAdress + ";" + version + ";" + "thePCName" + ";" + "0" + ";" + dominio;
         GetClau gcWS = new GetClau();
         var resp = gcWS.CallGetClau(strEnvia);

         //Ejemplo de registros de la tabla serials retornada
         //Cada registro está separado por "#" y cada campo por ";"
         //strRet = "4;PSANTOS;;B1EF0F30;2020-12-31 00:00:00;1;ASG;375;0;Un eventual mensaje en la BD;1;;ASG-PC02(eariño);-4;9999999;1;ABCD#2000;PSANTOS;;87AF8170;2020-12-31 00:00:00;1;ASG;375;0;Un eventual mensaje en la BD;1;;ASG-PC31(Administrador);0;9999999;1;ABCD"
         //posiciones0    1    2    3            4           5   6  7  8              9              10 11   12           13    14   15 16   0      1    2     3            4          5  6   7  8              9              10 11   12                  13  14    15  16
         //var strarrayret1 ="";


         var strarrayret1 = resp.Split('#');
         string resultString = "";
         foreach (string astring in strarrayret1)
         {
            var strArray2 = astring.Split(';');
            if (strArray2[16] == pw) 
            {resultString = resultString + astring + '#';
            }
            else
            {
            }
         }
         //Elimina el ultimo cardinal
         if (resultString.Length > 1)
         { resultString = resultString.Substring(0, resultString.Length - 1); }
         else
         { resultString = ""; }
            
         return resultString;

      }

      internal double[] GetU()
      {
         double[] u = new double[100];
         u[1] = -2.32634787404084;
         u[2] = -2.05374891063182;
         u[3] = -1.88079360815125;
         u[4] = -1.75068607125217;
         u[5] = -1.64485362695147;
         u[6] = -1.55477359459685;
         u[7] = -1.47579102817917;
         u[8] = -1.40507156030964;
         u[9] = -1.34075503369022;
         u[10] = -1.2815515655446;
         u[11] = -1.22652812003661;
         u[12] = -1.17498679206609;
         u[13] = -1.1263911290388;
         u[14] = -1.08031934081496;
         u[15] = -1.03643338949379;
         u[16] = -0.99445788320975;
         u[17] = -0.954165253146195;
         u[18] = -0.915365087842815;
         u[19] = -0.877896295051228;
         u[20] = -0.841621233572915;
         u[21] = -0.806421247018241;
         u[22] = -0.772193214188685;
         u[23] = -0.738846849185214;
         u[24] = -0.706302562840087;
         u[25] = -0.674489750196082;
         u[26] = -0.643345405392917;
         u[27] = -0.612812991016627;
         u[28] = -0.582841507271216;
         u[29] = -0.553384719555673;
         u[30] = -0.524400512708041;
         u[31] = -0.495850347347454;
         u[32] = -0.467698799114508;
         u[33] = -0.439913165673234;
         u[34] = -0.412463129441405;
         u[35] = -0.385320466407568;
         u[36] = -0.358458793251194;
         u[37] = -0.331853346436817;
         u[38] = -0.305480788099397;
         u[39] = -0.279319034447454;
         u[40] = -0.2533471031358;
         u[41] = -0.22754497664115;
         u[42] = -0.201893479141851;
         u[43] = -0.176374164780861;
         u[44] = -0.150969215496777;
         u[45] = -0.125661346855074;
         u[46] = -0.10043372051147;
         u[47] = -0.0752698620998299;
         u[48] = -0.0501535834647337;
         u[49] = -0.0250689082587111;
         u[50] = 0;
         u[51] = 0.0250689082587111;
         u[52] = 0.0501535834647337;
         u[53] = 0.0752698620998299;
         u[54] = 0.10043372051147;
         u[55] = 0.125661346855074;
         u[56] = 0.150969215496777;
         u[57] = 0.176374164780861;
         u[58] = 0.201893479141851;
         u[59] = 0.227544976641149;
         u[60] = 0.2533471031358;
         u[61] = 0.279319034447454;
         u[62] = 0.305480788099397;
         u[63] = 0.331853346436817;
         u[64] = 0.358458793251194;
         u[65] = 0.385320466407568;
         u[66] = 0.412463129441405;
         u[67] = 0.439913165673234;
         u[68] = 0.467698799114508;
         u[69] = 0.495850347347453;
         u[70] = 0.524400512708041;
         u[71] = 0.553384719555673;
         u[72] = 0.582841507271216;
         u[73] = 0.612812991016627;
         u[74] = 0.643345405392917;
         u[75] = 0.674489750196082;
         u[76] = 0.706302562840087;
         u[77] = 0.738846849185214;
         u[78] = 0.772193214188685;
         u[79] = 0.806421247018241;
         u[80] = 0.841621233572915;
         u[81] = 0.877896295051229;
         u[82] = 0.915365087842813;
         u[83] = 0.954165253146195;
         u[84] = 0.99445788320975;
         u[85] = 1.03643338949379;
         u[86] = 1.08031934081496;
         u[87] = 1.1263911290388;
         u[88] = 1.17498679206609;
         u[89] = 1.22652812003661;
         u[90] = 1.2815515655446;
         u[91] = 1.34075503369022;
         u[92] = 1.40507156030963;
         u[93] = 1.47579102817917;
         u[94] = 1.55477359459685;
         u[95] = 1.64485362695147;
         u[96] = 1.75068607125217;
         u[97] = 1.88079360815125;
         u[98] = 2.05374891063182;
         u[99] = 2.32634787404084;


         return u;
      }
   }

   public class cWSArcos
   {
      public string IDArc { get; set; }
      public string NINI { get; set; }
      public string NFin { get; set; }
      public int NTubosCandidatos { get; set; }
      public int SYSTAG { get; set; }
      public string Tipus { get; set; }
   }

   public class cWSNodos
   {
      public string IDNode { get; set; }
      public string Tipus { get; set; }
      public int sysTag { get; set; }
      public double PMin { get; set; }
      public double PMAx { get; set; }
      public bool PL_NodoConsolidado { get; set; }
   }
}
