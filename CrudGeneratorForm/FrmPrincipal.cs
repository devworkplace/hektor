using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrudGeneratorForm
{
    public partial class FrmPrincipal : Form
    {
        private Util util;

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            btnMarcarTabelas.Text = "Marcar Todas";
            if (!string.IsNullOrEmpty(txtConexao.Text))
            {
                cbxTabelas.Items.Clear();
                util = new Util(txtConexao.Text);
                util.PovoarCbxTabelas(cbxTabelas);
            }
        }

        private void btnGerar_Click(object sender, EventArgs e)
        {
            if (util != null && cbxTabelas.CheckedItems != null && cbxTabelas.CheckedItems.Count > 0)
            {
                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string path = dialog.SelectedPath;
                        if (!string.IsNullOrEmpty(path))
                        {
                            path = string.Format("{0}\\CRUDGenerator-{1}", path, util.FormatarDataParaDiretorioRaiz());
                            if (util.CriarDiretorio(path) != null)
                            {
                                string pathModels = string.Format("{0}\\Models", path);
                                string pathTO = string.Format("{0}\\Models\\TO", path);
                                string pathDAL = string.Format("{0}\\Models\\DAL", path);
                                string pathControllers = string.Format("{0}\\Controllers", path);
                                string pathViews = string.Format("{0}\\Views", path);
                                string pathScripts = string.Format("{0}\\Scripts", path);
                                string pathScriptsCustom = string.Format("{0}\\Scripts\\custom", path);

                                util.LimparDiretorio(pathModels);
                                util.LimparDiretorio(pathControllers);
                                util.LimparDiretorio(pathViews);
                                util.LimparDiretorio(pathScripts);

                                DirectoryInfo dirModels = util.CriarDiretorio(pathModels);
                                DirectoryInfo dirTO = util.CriarDiretorio(pathTO);
                                DirectoryInfo dirDAL = util.CriarDiretorio(pathDAL);
                                DirectoryInfo dirControllers = util.CriarDiretorio(pathControllers);
                                DirectoryInfo dirViews = util.CriarDiretorio(pathViews);
                                DirectoryInfo dirScripts = util.CriarDiretorio(pathScripts);
                                DirectoryInfo dirScriptsCustom = util.CriarDiretorio(pathScriptsCustom);

                                if (dirModels != null
                                    && dirTO != null
                                    && dirDAL != null
                                    && dirControllers != null
                                    && dirViews != null
                                    && dirScripts != null
                                    && dirScriptsCustom != null)
                                {
                                    Tabela tabela;
                                    string pathViewTabela;
                                    DirectoryInfo dirViewTabela;
                                    foreach (String nomeTabela in cbxTabelas.CheckedItems)
                                    {
                                        pathViewTabela = string.Format("{0}\\Views\\{1}", path, nomeTabela);
                                        try
                                        {
                                            dirViewTabela = Directory.CreateDirectory(pathViewTabela);

                                            tabela = new Tabela(txtNameSpace.Text, nomeTabela, pathTO, pathDAL, pathControllers, pathViewTabela, pathScriptsCustom, util);
                                            tabela.CriarArquivoTO();
                                            tabela.CriarArquivoDAL();
                                            tabela.CriarArquivoController();
                                            tabela.CriarArquivoView();
                                            tabela.CriarArquivoJavaScript();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message);
                                        }
                                    }
                                    MessageBox.Show("Sucesso");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Nenhuma tabela foi selecionada");
            }
        }

        private void btnMarcarTabelas_Click(object sender, EventArgs e)
        {
            if (cbxTabelas.Items.Count > 0)
            {
                bool status;
                if ("Marcar Todas".Equals(btnMarcarTabelas.Text))
                {
                    status = true;
                    btnMarcarTabelas.Text = "Desmarcar Todas";
                }
                else
                {
                    status = false;
                    btnMarcarTabelas.Text = "Marcar Todas";
                }
                for (int i = 0; i < cbxTabelas.Items.Count; i++)
                {
                    cbxTabelas.SetItemChecked(i, status);
                }
            }
            else
            {
                MessageBox.Show("Nenhuma tabela foi encontrada");
            }
        }
    }
}
