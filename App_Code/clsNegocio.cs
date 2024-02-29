using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;
using Credipaz_Lib;

/// <summary>
/// Summary description for clsNegocio
/// </summary>
public class clsNegocio
{
    wsServicios.Service wsServicios = new wsServicios.Service();

    MilService.Service1Client wsMilService = new MilService.Service1Client();

    Credipaz_Lib.Datos libCpz = new Credipaz_Lib.Datos();

    DataTable dtUsuario = new DataTable();

    DataTable dt = new DataTable();
    DataTable dtAdi = new DataTable();

    XmlDocument xdDocumento;

    XmlElement xeAdicional;
    XmlElement xeCuenta;
    XmlElement xeDisponible;
    XmlElement xeDireccion;
    XmlElement xeError;
    XmlElement xeErrorMensaje;
    XmlElement xeFechaConsulta;
    XmlElement xeLocalidad;
    XmlElement xeNombre;
    XmlElement xeNroDocumento;
    XmlElement xePosicionFiscal;
    XmlElement xeProvincia;
    XmlElement xePuedeOperar;
    XmlElement xePuntos;
    XmlElement xeRegistro;
    XmlElement xeRegistroEntrada;
    XmlElement xeRegistroSalida;
    XmlElement xeSocioRedondo;
    XmlElement xeTiempoRespuesta;
    XmlElement xeTipo;
    XmlElement xeTipoDocumento;
    XmlElement xeValor;

    String Mascara_Fecha = "yyyy-MM-dd HH:mm:ss";

    DateTime dFechaInicio;
    DateTime dFechaFin;
    DateTime dFechaX;

	public clsNegocio()
	{
        
	}

    public string AutorizarCompra(string sUsuario,
                                  string sPassword,
                                  string sPin,
                                  int nDoc,
                                  string sNombre,
                                  string sVCode, 
                                  string sFechaVto,
                                  decimal nImporte)
    {
        string sResultado = "";
       
        string sConexion = "Data Source=SVRDBCP01;Initial Catalog=dbCentral;UID=SA;PWD=SQLCREDIPAZ25;Integrated Security=False;Connection Timeout=60";

        if (this.ValidarUsuario(sConexion,
                                sUsuario,
                                sPassword,
                                sPin))
        {
            sResultado = wsMilService.autorizarCompra(nDoc,
                                                      sNombre,
                                                      sVCode,
                                                      sFechaVto,
                                                      nImporte);
        }
        else
        {
            sResultado = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><operacion><respuesta resultado=\"Usuario y/ó password ó pin son incorrectos\" /></operacion>";
        }
   
        return sResultado;
    }

    public string ConsultarDisponible(string sUsuario,
                                      string sPassword,
                                      string sPin,
                                      string sTipoConsulta,
                                      string sValor)
    {
        string sResultado = "";
        string sDocumentoXML = "";
  
        XmlDocument sDocXML = new XmlDocument();
        XmlNodeList xnlLista;

        try
        {
            CultureInfo ci = new CultureInfo("es-AR");

            xdDocumento = new XmlDocument();
            xeRegistro = xdDocumento.CreateElement("Registro");
            xeRegistroEntrada = xdDocumento.CreateElement("RegistroEntrada");
            xeFechaConsulta = xdDocumento.CreateElement("FechaConsulta");
            xeTipo = xdDocumento.CreateElement("Tipo");
            xeValor = xdDocumento.CreateElement("Valor");
            xeRegistroSalida = xdDocumento.CreateElement("RegistroSalida");
            xeCuenta = xdDocumento.CreateElement("Cuenta");
            xeAdicional = xdDocumento.CreateElement("Adicional");
            xePuedeOperar = xdDocumento.CreateElement("PuedeOperar");
            xeDisponible = xdDocumento.CreateElement("Disponible");
            xePuntos = xdDocumento.CreateElement("Puntos");
            xeSocioRedondo = xdDocumento.CreateElement("SocioRedondo");
            xeNombre = xdDocumento.CreateElement("Nombre");
            xeTipoDocumento = xdDocumento.CreateElement("TipoDocumento");
            xeNroDocumento = xdDocumento.CreateElement("NroDocumento");
            xeDireccion = xdDocumento.CreateElement("Direccion");
            xeLocalidad = xdDocumento.CreateElement("Localidad");
            xeProvincia = xdDocumento.CreateElement("Provincia");
            xePosicionFiscal = xdDocumento.CreateElement("PosicionFiscal");
            xeError = xdDocumento.CreateElement("Error");
            xeErrorMensaje = xdDocumento.CreateElement("ErrorMensaje");
            xeTiempoRespuesta = xdDocumento.CreateElement("TiempoRespuesta");

            xeFechaConsulta.InnerText = string.Format("{0:" + Mascara_Fecha + "}", DateTime.Now);
            xeTipo.InnerText = sTipoConsulta;
            xeValor.InnerText = sValor;

            xeRegistroEntrada.AppendChild(xeFechaConsulta);
            xeRegistroEntrada.AppendChild(xeTipo);
            xeRegistroEntrada.AppendChild(xeValor);
            xeRegistro.AppendChild(xeRegistroEntrada);

            xeCuenta.InnerText = "";
            xeAdicional.InnerText = "";
            xePuedeOperar.InnerText = "0";
            xeDisponible.InnerText = "0.00";
            xePuntos.InnerText = "0";
            xeSocioRedondo.InnerText = "0";
            xeNombre.InnerText = "";
            xeTipoDocumento.InnerText = "";
            xeNroDocumento.InnerText = "";
            xeDireccion.InnerText = "";
            xeLocalidad.InnerText = "";
            xeProvincia.InnerText = "";
            xePosicionFiscal.InnerText = "CONSUMIDOR FINAL";
            xeError.InnerText = "0";
            xeErrorMensaje.InnerText = "";
            
            dFechaInicio = DateTime.Now;

            string sConexion = "Data Source=SVRDBCP01;Initial Catalog=dbCentral;UID=SA;PWD=SQLCREDIPAZ25;Integrated Security=False;Connection Timeout=60";

            if (this.ValidarUsuario(sConexion,
                                    sUsuario,
                                    sPassword,
                                    sPin))
            {
                dt = new DataTable();

                SqlParameterCollection prmc = new SqlCommand().Parameters;

                prmc.Add("@sTipoConsulta ", SqlDbType.Char, 1).Value = sTipoConsulta;
                prmc.Add("@sValor", SqlDbType.VarChar, 16).Value = sValor;

                dt = libCpz.ConsultarSP(sConexion, "mil_Buscar_Cliente", prmc);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        xeCuenta.InnerText = dr["sCodigo"].ToString();
                        xeAdicional.InnerText = dr["nAdicional"].ToString();
                        xeNombre.InnerText = dr["sNombre"].ToString();
                        xeTipoDocumento.InnerText = dr["sLKDocTipo"].ToString();
                        xeNroDocumento.InnerText = dr["nDoc"].ToString();
                        xeDireccion.InnerText = dr["sDomicilio"].ToString();
                        xeLocalidad.InnerText = dr["sLKDomiLocalidad"].ToString();
                        xeProvincia.InnerText = dr["sProvincia"].ToString();
                    }
                }

                if (xeCuenta.InnerText != "")
                {
                    sDocumentoXML = wsServicios.Consultar_Disponible("1",
                                                                     xeCuenta.InnerText,
                                                                     "00",
                                                                     "00");
                }
                else
                {
                    sDocumentoXML = wsServicios.Consultar_Disponible(xeTipo.InnerText,
                                                                     xeValor.InnerText,
                                                                     "00",
                                                                     "00");
                }

                if (sDocumentoXML != "")
                {
                    sDocXML.LoadXml(sDocumentoXML);

                    xnlLista = sDocXML.GetElementsByTagName("RegistroSalida");

                    foreach (XmlNode xnNodo in xnlLista)
                    {
                        if (xnNodo.HasChildNodes == true)
                        {
                            for (int i = 0; i < xnNodo.ChildNodes.Count; i++)
                            {
                                switch (xnNodo.ChildNodes.Item(i).Name)
                                {
                                    case "Disponible":
                                        xeDisponible.InnerText = xnNodo.ChildNodes.Item(i).InnerText;
                                        break;

                                    case "PuedeOperar":
                                        xePuedeOperar.InnerText = xnNodo.ChildNodes.Item(i).InnerText;
                                        break;

                                    case "PuntosDia":
                                        xePuntos.InnerText = xnNodo.ChildNodes.Item(i).InnerText;
                                        break;

                                    case "SocioRedondo":
                                        xeSocioRedondo.InnerText = xnNodo.ChildNodes.Item(i).InnerText;
                                        break;

                                    case "Error":
                                        xeError.InnerText = xnNodo.ChildNodes.Item(i).InnerText;
                                        break;

                                    case "ErrorMensaje":
                                        xeErrorMensaje.InnerText = xnNodo.ChildNodes.Item(i).InnerText;
                                        break;

                                    case "Cuenta":
                                        if (xeCuenta.InnerText == "")
                                        {
                                            xeCuenta.InnerText = xnNodo.ChildNodes.Item(i).InnerText;
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    if (xeCuenta.InnerText != "")
                    {
                        if (xeTipo.InnerText == "2")
                        {
                            xeNroDocumento.InnerText = xeValor.InnerText;

                            if (xeAdicional.InnerText == "")
                            {
                                SqlParameterCollection prmcAdi = new SqlCommand().Parameters;

                                prmcAdi.Add("@sCuenta", SqlDbType.Char, 10).Value = xeCuenta.InnerText;
                                prmcAdi.Add("@nDocumento", SqlDbType.Int).Value = xeNroDocumento.InnerText;

                                dtAdi = libCpz.ConsultarSP(sConexion, "mil_Buscar_Adicional", prmcAdi);

                                foreach (DataRow drAdi in dtAdi.Rows)
                                {
                                    xeAdicional.InnerText = drAdi["nAdicional"].ToString();
                                    xeNombre.InnerText = drAdi["sNombre"].ToString();
                                    xeTipoDocumento.InnerText = drAdi["sLKDocTipo"].ToString();
                                }
                            }
                        }
                    }
                }


                if (xeError.InnerText != "0")
                {
                    if (Convert.ToInt32(xeSocioRedondo.InnerText) > 0)
                    {
                        xeError.InnerText = "0";
                    }
                    else
                    {
                        xePuedeOperar.InnerText = "1";
                    }

                    switch (xeError.InnerText)
                    {
                        case "ERR02":
                              xeError.InnerText = "ERR99";
                              xeErrorMensaje.InnerText = "Cliente inexistente";
                              break;

                        case "CTA01":
                              xeError.InnerText = "0";
                              xeErrorMensaje.InnerText = "Cliente inexistente";
                              break;
                    }
                }
            }
            else
            {
                xeError.InnerText = "9999";
                xeErrorMensaje.InnerText = "Usuario y/ó password ó pin son incorrectos";                                          
            }

            dFechaFin = DateTime.Now;

            xeTiempoRespuesta.InnerText = (dFechaFin.Second - dFechaInicio.Second).ToString();

            xeRegistroSalida.AppendChild(xeCuenta);
            xeRegistroSalida.AppendChild(xeAdicional);
            xeRegistroSalida.AppendChild(xePuedeOperar);
            xeRegistroSalida.AppendChild(xeDisponible);
            xeRegistroSalida.AppendChild(xePuntos);
            xeRegistroSalida.AppendChild(xeSocioRedondo);
            xeRegistroSalida.AppendChild(xeNombre);
            xeRegistroSalida.AppendChild(xeTipoDocumento);
            xeRegistroSalida.AppendChild(xeNroDocumento);
            xeRegistroSalida.AppendChild(xeDireccion);
            xeRegistroSalida.AppendChild(xeLocalidad);
            xeRegistroSalida.AppendChild(xeProvincia);
            xeRegistroSalida.AppendChild(xePosicionFiscal);
            xeRegistroSalida.AppendChild(xeError);
            xeRegistroSalida.AppendChild(xeErrorMensaje);
            xeRegistroSalida.AppendChild(xeTiempoRespuesta);
            xeRegistro.AppendChild(xeRegistroSalida);
            xdDocumento.AppendChild(xeRegistro);

            sResultado = xdDocumento.InnerXml;

        }
        catch (Exception ex)
        {
            sResultado = "";
        }

        return sResultado;
    }

    private bool ValidarUsuario(string sConexion,
                                string sUsuario,
                                string sPassword,
                                string sPin)
    {
        bool bResultado = false;

        dt = new DataTable();

        SqlParameterCollection prmc = new SqlCommand().Parameters;

        prmc.Add("@sUsuario", SqlDbType.VarChar, 15).Value = sUsuario;
     
        dt = libCpz.ConsultarSP(sConexion, "mil_Validar_Usuario", prmc);

        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (string.Compare(dr["sPassword"].ToString(), sPassword) == 0)
                {
                    if (string.Compare(dr["sPin"].ToString(), sPin) == 0)
                    {
                        bResultado = true;
                    }
                } 
            }
        }

        return bResultado;
    }

    public string RegistrarVentaMil(string sUsuario,
                                    string sPassword,
                                    string sPin,
                                    string xmlVenta)
    {
        string sResultado = "";

        string sConexion = "Data Source=SVRDBCP01;Initial Catalog=dbCentral;UID=SA;PWD=SQLCREDIPAZ25;Integrated Security=False;Connection Timeout=60";

        if (this.ValidarUsuario(sConexion,
                                sUsuario,
                                sPassword,
                                sPin))
        {
            sResultado = wsMilService.RegistrarVentaMil(xmlVenta);
        }
        else
        {
            sResultado = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><operacion><respuesta resultado=\"Usuario y/ó password ó pin son incorrectos\" /></operacion>";
        }

        return sResultado;
    }

    public string RegistrarVentaGarantiaEX(string sUsuario,
                                           string sPassword,
                                           string sPin,
                                           string xmlGEX)
    {
        string sResultado = "";

        string sConexion = "Data Source=SVRDBCP01;Initial Catalog=dbCentral;UID=SA;PWD=SQLCREDIPAZ25;Integrated Security=False;Connection Timeout=60";

        if (this.ValidarUsuario(sConexion,
                                sUsuario,
                                sPassword,
                                sPin))
        {
            sResultado = wsMilService.RegistrarVentaGarantiaEX(xmlGEX);
        }
        else
        {
            sResultado = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><operacion><respuesta resultado=\"Usuario y/ó password ó pin son incorrectos\" /></operacion>";
        }

        return sResultado;
    }

 }
