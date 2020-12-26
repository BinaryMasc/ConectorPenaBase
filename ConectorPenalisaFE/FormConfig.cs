using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConfiguracionNS;

namespace ConectorPenalisaFE
{
    public partial class FormConfig : Form
    {
        public FormConfig()
        {
            InitializeComponent();
        }

        private void FormConfig_Load(object sender, EventArgs e)
        {

        }


        private void Seguridad_Integrada_BBDD_CheckedChanged(object sender, EventArgs e)
        {
            if (Seguridad_Integrada_BBDD.Checked)
            {
                Usuario_BBDD.Enabled = false;
                Password_BBDD.Enabled = false;
            }
            else
            {
                Usuario_BBDD.Enabled = true;
                Password_BBDD.Enabled = true;
            }
        }

        private void Guardar_Click(object sender, EventArgs e)
        {

            //  Verificamos que los campos estén llenos
            if (hayCampoVacio(this))
            {
                MessageBox.Show("Debe llenar todos los campos.");
                return;
            }


            FileConfig newconfig = new FileConfig();

            //  InfoAut DBNet:
            newconfig.idEmpresaDBNet = this.idEmpresaDBNet.Text;
            newconfig.UserEmpresaDBNet = this.UserEmpresaDBNet.Text;
            newconfig.AutorizacionEmpresaDBNet = this.AutorizacionEmpresaDBNet.Text;
            newconfig.URLWSDBNet = this.URLWSDBNet.Text;

            //  InfoProc:
            newconfig.idArea_InfoProc = this.idArea_InfoProc.Text;
            newconfig.signDoc_InfoProc = this.signDoc_InfoProc.Text;
            newconfig.asgCorr_InfoProc = this.asgCorr_InfoProc.Text;
            newconfig.genXml_InfoProc = this.genXml_InfoProc.Text;
            newconfig.saveDoc_InfoProc = this.saveDoc_InfoProc.Text;

            //  Dirección:
            newconfig.TipoDireccion = this.TipoDireccion.Text;
            newconfig.IdDUNS = this.IdDUNS.Text;
            newconfig.ApartadoPostal = this.ApartadoPostal.Text;
            newconfig.Direccion = this.Direccion.Text;
            newconfig.Area = this.Area.Text;
            newconfig.Ciudad = this.Ciudad.Text;
            newconfig.Departamento = this.Departamento.Text;
            newconfig.CodigoDepartamento = this.CodigoDepartamento.Text;
            newconfig.CodigoPais = this.CodigoPais.Text;
            newconfig.NombrePais = this.NombrePais.Text;

            //  Contacto
            newconfig.TipoContacto = this.TipoContacto.Text;
            newconfig.NombreContacto = this.NombreContacto.Text;
            newconfig.TelefonoContacto = this.TelefonoContacto.Text;
            newconfig.MailContacto = this.MailContacto.Text;
            newconfig.DescripcionContacto = this.DescripcionContacto.Text;

            //  Base de datos
            newconfig.Server_BBDD = this.Server_BBDD.Text;
            newconfig.Nombre_BBDD = this.Nombre_BBDD.Text;
            newconfig.NombreVistaFAC = this.NombreVistaFAC.Text;
            newconfig.NombreVistaNotas = this.NombreVistaNotas.Text;
            newconfig.Seguridad_Integrada_BBDD = this.Seguridad_Integrada_BBDD.Checked;
            newconfig.Usuario_BBDD = this.Usuario_BBDD.Text;
            newconfig.Password_BBDD = this.Password_BBDD.Text;

            //  Config General Conector
            newconfig.IntervaloTimerConector = Convert.ToInt32(this.IntervaloTimerConector.Text);
            newconfig.GenerarJsonDocumentosConector = this.GenerarJsonDocumentosConector.Checked;


            newconfig.RutaGuardadoJsonDocumentosFACConector = "C:\\Users\\"+Environment.UserName+"\\Documents\\ConectorFE\\Facturas_Generadas\\";
            newconfig.RutaGuardadoJsonDocumentosNOTASConector = "C:\\Users\\"+Environment.UserName+"\\Documents\\ConectorFE\\Notas_Generadas\\";

            Configuracion.GuardarConfiguracion(newconfig);


            this.Close();
        }

        private bool hayCampoVacio(Form formulario)
        {
            foreach(Control control in formulario.Controls)
            {
                if (control is TextBox && control.Text == String.Empty && control.Name != "Usuario_BBDD" && control.Name != "Password_BBDD") 
                {
                    return true;
                }
            }
            return false;
        }
    }
}
