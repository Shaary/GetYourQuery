using GetYourQuery.Core;
using System;
using System.Windows.Forms;
using System.Linq;
using System.ComponentModel;
using System.Data;

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

            this.repository = new Repository();
            this.procType = "";
            this.storedProcedureName = "";
            this.databaseName = "AmazingDb";
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

            var tablesTable = repository.TableNamesGet(schema);

            var storedProcQuery = GetStoredProcQueryType(procType, tablesTable, storedProcBox.Text, schema);

            var tableName = StoredProcQuery.TableNameGet(storedProcBox.Text);

            if (storedProcQuery.IsTableExists(tableName))
            {
                var parametersTable = repository.ParametersTableGet(storedProcedureName, schema);

                storedProcQuery.ParamaterNamesSet(parametersTable);
                //storedProcQuery.ParametersDataGenerate();

                var dict = storedProcQuery.TableAndColumnNamesGet(schema);
                var data = repository.IdParametersDataGet(dict);

                //TODO: add scroll bar to query text
                queryTextBox.Text = storedProcQuery.QueryGet(schema, data);
            }
            else
            {
                queryTextBox.Text = "Sorry, I couldn't find underlying table";
            }
            

        }

        private IStoredProcQuery GetStoredProcQueryType(string procType, DataTable tablesTable, string storedProcedureName, string schemaName)
        {
            switch(procType)
            {
                case "Add":
                    return new AddStoredProcQuery(tablesTable, storedProcedureName, schemaName);
                case "Update":
                    return new UpdateStoredProcQuery(tablesTable, storedProcedureName, schemaName);
                case "Delete":
                    return new DeleteStoredProcQuery(tablesTable, storedProcedureName, schemaName);
                case "Get":
                    return new GetStoredProcQuery(tablesTable, storedProcedureName, schemaName);
                default:
                    {
                        throw new Exception("Type wasn't found");
                    }
            }
        }
    }
}
