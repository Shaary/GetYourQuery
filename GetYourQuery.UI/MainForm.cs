using GetYourQuery.Core;
using GetYourQuery.Data;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace GetYourQuery.UI
{
    public partial class MainForm : Form
    {
        private IRepository repository;

        private string procType;
        private string storedProcedureName;
        private string databaseName;
        private string server;

        public MainForm()
        {
            InitializeComponent();

            var stringBuilder = new SqlConnectionStringBuilder(System.Configuration.ConfigurationManager
                                        .ConnectionStrings["unify"].ConnectionString);

            var connectionString = stringBuilder.ConnectionString;

            this.repository = new Repository(connectionString);
            this.databaseName = stringBuilder.InitialCatalog;
            this.server = stringBuilder.DataSource;
        }

        private void LoadShemaBox(ComboBox schemaBox)
        {
            schemaBox.DataSource = null;
            schemaBox.Items.Clear();

            schemaBox.DataSource = repository.SchemaList;
        }

        private void LoadStoreProcBox(ComboBox storedProcBox, string schema, string database, string procType)
        {
            storedProcBox.DataSource = null;
            storedProcBox.Items.Clear();
            queryTextBox.Clear();

            var storedProcNames = repository.StoredProcedureNamesGet(schema, database, procType);
            storedProcBox.DataSource = storedProcNames;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ReloadDataFields();
            LoadLabelNames();
        }

        private void LoadLabelNames()
        {
            databaseLabel.Text = $"Database: {databaseName}";
            serverLabel.Text = $"Server: {server}";
        }

        private void ReloadDataFields()
        {
            try
            {
                LoadShemaBox(schemaBox);
                LoadStoreProcBox(storedProcBox, GetSelectedSchema(schemaBox), databaseName, procType);

            }
            catch (Exception e)
            {
                MessageBox.Show("Problem connecting to database\n" + e.ToString());
            }
        }

        private void StoredProcedureComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            queryTextBox.Clear();
            ComboBox comboBox = sender as ComboBox;
            storedProcedureName = comboBox.Text.ToString();
        }

        private void SchemaComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            LoadStoreProcBox(storedProcBox, GetSelectedSchema(schemaBox), databaseName, procType);
        }

        private string GetSelectedSchema(ComboBox comboBox)
        {
            return comboBox.SelectedValue as string;
        }

        private void ProcTypeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton groupBox = sender as RadioButton;
            procType = groupBox.Text.ToString();

            LoadStoreProcBox(storedProcBox, GetSelectedSchema(schemaBox), databaseName, procType);
        }

        private void FindButton_Click(object sender, EventArgs e)
        {
            try
            {
                var schema = GetSelectedSchema(schemaBox);
                var storedProc = storedProcBox.Text;

                var tableName = NameModifier.TableNameGet(storedProc);

                if (repository.IsTableExists(tableName, schema))
                {
                    var data = repository.DataGet(storedProc, schema, procType);

                    queryTextBox.Text = StoredProcQuery.QueryGet(schema, storedProc, data);
                }
                else
                {
                    queryTextBox.Text = "Sorry, I couldn't find underlying table";
                }
            }
            catch (Exception ex)
            {
                queryTextBox.Text = ex.ToString();
            }
            
        }
    }
}
