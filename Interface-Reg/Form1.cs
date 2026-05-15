using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Library de MySQL
using MySql.Data.MySqlClient;

namespace Interface_Reg
{
    public partial class Form1 : Form
    {

        // String de conexão com o banco de dados - atualmente rodando em Docker
        string connection = "server=localhost;port=3307;database=cadastro_db;uid=usuario;pwd=senha123;";

        // Método de busca de itens no DB e exibição no Grid
        private void ListItems()
        {
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    conn.Open(); // .Open -> Abre o banco de dados com a conexão settada

                    // Comando SQL para seleção de todos os itens
                    string sql = "SELECT id, nome, descricao, quantidade FROM itens";

                    // Mapeamento e ponte para obentção de dados SQL
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);

                    // Armazenamento de dados na memória
                    DataTable tabela = new DataTable();

                    // Preenchimento da tabela com os dados recebidos do banco
                    adapter.Fill(tabela);

                    // .DataSouce -> Define de onde vêm os dados recebidos
                    dgvItens.DataSource = tabela;
                }
                catch(Exception ex) 
                {
                    MessageBox.Show("Erro ao listar itens: " + ex.Message);
                }
            }

        }

        // Limpeza de campos da tela
        private void LimparCampos()
        {
            txtNome.Clear();
            txtDescricao.Clear();
            txtQuantidade.Clear();

            // .Focus define a entrada a ser priorizada
            txtNome.Focus();
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            // Verificação de campos obrigatórios preenchidos
            if (txtNome.Text == "" || txtQuantidade.Text == "")
            {
                MessageBox.Show("Preencha os dados necessários: nome, quantidade");
                return;
            }

            // Verificação de inserção de número inteiro na quantidade digitada
            if (!int.TryParse(txtQuantidade.Text, out int quantidade))
            {
                MessageBox.Show("Digite uma quantidade válida.");
                return;
            }

            // Criação de Conexão com o banco
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    // Abre conexão
                    conn.Open();

                    // Comando SQL para inserção de dados
                    string sql = "INSERT INTO itens (nome, descricao, quantidade) VALUES (@nome, @descricao, @quantidade)";

                    // Cria o comando SQL - MySqlCommand executa os Sql
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    // Substitui os parâmetros pelos valores inseridos da interface
                    // Pega os parâmetros vindos de um SQL, e adiciona valores: @nome <- -> txtNome.Text
                    cmd.Parameters.AddWithValue("@nome", txtNome.Text);
                    cmd.Parameters.AddWithValue("@descricao", txtDescricao.Text);
                    cmd.Parameters.AddWithValue("@quantidade", txtQuantidade.Text);

                    // Executa a inserção na conexão e retorna a quantia de linhas afetadas
                    cmd.ExecuteNonQuery();

                    MessageBox.Show(
                        "O item foi cadastrado com sucesso! " + txtNome.Text,
                        "Confirmação",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.None
                    );


                    // Limpa os campos
                    LimparCampos();

                    // Atualiza a tabela na interface
                    ListItems();
                }
                catch (Exception ex) 
                {
                    MessageBox.Show("Erro ao cadastrar item: " + ex.Message);
                }
            }
        }

        private void btnListar_Click(object sender, EventArgs e)
        {
            // Chama o método de listagem
            ListItems();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            // Verifica a seleção de uma linha
            if (dgvItens.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um item para excluir.");
                return;
            }

            // Pega o ID do item selecionado
            // .SelectedRows[0] -> Reconhece a linha 0 como selecionada
            // .Cells["id"] -> Identifica que, na linha celecionada, é obtido/definido o valor da célula id 
            int id = Convert.ToInt32(dgvItens.SelectedRows[0].Cells["id"].Value);

            DialogResult resposta = MessageBox.Show(
                "Deseja realmente excluir este item?",
                "Confirmação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // Caso selecione a exclusão:
            if (resposta == DialogResult.Yes)
            {
                using (MySqlConnection conn = new MySqlConnection(connection))
                {
                    try
                    {
                        conn.Open();

                        // Execução de deletar via ID
                        string sql = "DELETE FROM itens WHERE id = @id";

                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@id", id);

                        // Método que faz com que não haja retorno de qualquer valor
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Item excluído com sucesso.");

                        ListItems();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao excluir item: " + ex.Message);
                    }
                }
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            LimparCampos();
        }
    }
}
