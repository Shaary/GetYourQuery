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

        public MainForm()
        {
            InitializeComponent();

            //var connectionString = "Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=AmazingDb; Integrated Security=True;";

            var stringBuilder = new SqlConnectionStringBuilder(System.Configuration.ConfigurationManager
                                        .ConnectionStrings["unify"].ConnectionString);

            var connectionString = stringBuilder.ConnectionString;

            this.repository = new Repository(connectionString);
            this.databaseName = stringBuilder.InitialCatalog;
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
            var schema = GetSelectedSchema(schemaBox);
            var storedProc = storedProcBox.Text;

            var storedProcQuery = new StoredProcQuery();

            var tableName = NameModifier.TableNameGet(storedProc);

            if (repository.IsTableExists(tableName, schema))
            {
                var data = repository.DataGet(storedProc, schema, procType);

                //TODO: add scroll bar to query text
                queryTextBox.Text = storedProcQuery.QueryGet(schema, storedProc, data);
            }
            else
            {
                queryTextBox.Text = "Sorry, I couldn't find underlying table";
            }


        }

        //private IStoredProcQuery GetStoredProcQueryType(string procType, DataTable tablesTable, string storedProcedureName, string schemaName)
        //{
        //    switch(procType)
        //    {
        //        case "Add":
        //            return new AddStoredProcQuery(tablesTable, storedProcedureName, schemaName);
        //        case "Update":
        //            return new UpdateStoredProcQuery(tablesTable, storedProcedureName, schemaName);
        //        case "Delete":
        //            return new DeleteStoredProcQuery(tablesTable, storedProcedureName, schemaName);
        //        case "Get":
        //            return new GetStoredProcQuery(tablesTable, storedProcedureName, schemaName);
        //        default:
        //            {
        //                throw new Exception("Type wasn't found");
        //            }
        //    }
        //}
    }
}
