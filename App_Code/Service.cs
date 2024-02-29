using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

[WebService(Namespace = "https://aplicaciones.credipaz.com/wsMIL/wsMIL.asmx")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Service : System.Web.Services.WebService
{
    clsNegocio negObj = new clsNegocio();
    
    public Service () 
    {

    }

    [WebMethod(Description = "Autorizar Compra")]
    public string AutorizarCompra(string sUsuario,
                                  string sPassword,
                                  string sPin,
                                  int nDoc,
                                  string sNombre,
                                  string sVCode,
                                  string sFechaVto,
                                  decimal nImporte)
    {
        return negObj.AutorizarCompra(sUsuario,
                                      sPassword,
                                      sPin,
                                      nDoc,
                                      sNombre,
                                      sVCode,
                                      sFechaVto,
                                      nImporte);
    }

    [WebMethod(Description = "Consulta de disponible por: 1=Cuenta, 2=Documento ó 3=Tarjeta")]
    public string ConsultarDisponible(string sUsuario,
                                      string sPassword,
                                      string sPin,
                                      string sTipoConsulta,
                                      string sValor)
    {
        return negObj.ConsultarDisponible(sUsuario, 
                                          sPassword, 
                                          sPin, 
                                          sTipoConsulta, 
                                          sValor);       
    }

    [WebMethod(Description = "Registra Venta")]
    public string RegistrarVentaMil(string sUsuario,
                                    string sPassword,
                                    string sPin, 
                                    string xmlVenta)
    {
        return negObj.RegistrarVentaMil(sUsuario,
                                        sPassword,
                                        sPin,
                                        xmlVenta);
    }

    [WebMethod(Description = "Registra Venta Garantia Extendida")]
    public string RegistrarVentaGarantiaEX(string sUsuario,
                                           string sPassword,
                                           string sPin,
                                           string xmlGEX)
    {
        return negObj.RegistrarVentaGarantiaEX(sUsuario,
                                               sPassword,
                                               sPin,
                                               xmlGEX);
    }
}
