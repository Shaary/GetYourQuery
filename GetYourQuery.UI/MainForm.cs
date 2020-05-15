using GetYourQuery.Core;
using System;
using System.Windows.Forms;
using System.Linq;
using System.ComponentModel;

namespace GetYourQuery.UI
{
    public partial class MainForm : Form
    {
        private IRepository repository;

        private string procType;
        private string storedProcedureName;
        private string databaseName;
        private string connectionString;

        public MainForm()
        {
            InitializeComponent();
            this.connectionString = "Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=AmazingDb; Integrated Security=True;";

            this.repository = new Repository(connectionString);
            this.procType = "";
            this.storedProcedureName = "";
            this.databaseName = "AmazingDb";
        }

        public void btnFind_Click(object sender, EventArgs e)
        {

        }

        private void LoadShemaBox(ComboBox schemaBox)
        {
            schemaBox.DataSource = null;
            schemaBox.Items.Clear();

            var schemaNames = repository.SchemaNamesGet();
            schemaBox.DataSource = schemaNames;
        }

        private void LoadStoreProcBox(ComboBox storedProcBox, string schema, string database, string procType)
        {
            storedProcBox.DataSource = null;
            storedProcBox.Items.Clear();

            var storedProcNames = repository.StoredProcedureNamesGet(schema, database, procType);
            storedProcBox.DataSource = storedProcNames;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ReloadDataFields();
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
            var repository = new Repository(connectionString);
            var storedProcQuery = new StoredProcQuery();
            var schema = GetSelectedSchema(schemaBox);

            var dataTable = repository.ParametersTableGet(storedProcedureName, schema);
            repository.SchemaNamesGet();

            storedProcQuery.ParamaterNamesGet(dataTable);
            storedProcQuery.ParametersDataGenerate();

            //Sets storedProcsQuery tableNameTable to check for non-existing tables that will show up from params like ExternalUniqueId
            var tablesTable = repository.TableNamesGet(schema);
            storedProcQuery.TableNameTable = tablesTable;

            var dict = storedProcQuery.TableAndColumnNamesGet(schema);

            var data = repository.ParametersDataGet(dict);

            var query = storedProcQuery.QueryGet(schema, storedProcedureName, data);

            queryTextBox.Text = query;
        }


    }
}
