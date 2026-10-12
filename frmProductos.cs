using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

//========================================================================================
//Roberto Carlos Bonilla Molina, 260387, Tecnico en Ingenieria de Sistemas de Computación
//Emerson Isai Galeas Beltran , 260778 , Tecnico en Ingenieria de Sistemas de Computación
//Prueba Parcial: Programación I 
//Base de Datos: ParcialProgramacion1
//Sistema de Control de Productos
//========================================================================================

namespace ParcialProgramacion1
{
    public partial class Form1 : Form
    {
        string connectionString = "Server=.\\SQLEXPRESS; Database=ParcialProgramacion1; Trusted_Connection=True;";

        public Form1()
        {
            InitializeComponent();

            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.btnConsultar.Click += new System.EventHandler(this.btnConsultar_Click);
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.txtIdproducto.TextChanged += new System.EventHandler(this.txtIdproducto_TextChanged);
        }

        private void frmProductos_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void txtIdproducto_TextChanged(object sender, EventArgs e)
        {

        }

        private void CargarDatos()
        {
            string query = "SELECT IdProducto, Nombre, Categoria, Precio, Existencias FROM Productos";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al consultar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                MessageBox.Show("El nombre y la categoría son obligatorios.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("El precio debe ser un número válido mayor que cero.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtExistencias.Text, out int existencias) || existencias < 0)
            {
                MessageBox.Show("Las existencias deben ser un número entero igual o mayor que cero.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "INSERT INTO Productos (Nombre, Categoria, Precio, Existencias) VALUES (@Nombre, @Categoria, @Precio, @Existencias)";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Categoria", txtCategoria.Text.Trim());
                    cmd.Parameters.AddWithValue("@Precio", precio);
                    cmd.Parameters.AddWithValue("@Existencias", existencias);

                    try
                    {
                        con.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("¡Producto guardado correctamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LimpiarCampos();
                            CargarDatos();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocurrió un error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdproducto.Text) &&
                string.IsNullOrWhiteSpace(txtNombre.Text) &&
                string.IsNullOrWhiteSpace(txtCategoria.Text) &&
                string.IsNullOrWhiteSpace(txtPrecio.Text) &&
                string.IsNullOrWhiteSpace(txtExistencias.Text))
            {
                CargarDatos();
                MessageBox.Show("Mostrando todos los registros.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string query = "SELECT IdProducto, Nombre, Categoria, Precio, Existencias FROM Productos WHERE 1=1";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    if (!string.IsNullOrWhiteSpace(txtIdproducto.Text))
                    {
                        query += " AND IdProducto = @IdProducto";
                        cmd.Parameters.AddWithValue("@IdProducto", Convert.ToInt32(txtIdproducto.Text.Trim()));
                    }
                    if (!string.IsNullOrWhiteSpace(txtNombre.Text))
                    {
                        query += " AND Nombre LIKE @Nombre";
                        cmd.Parameters.AddWithValue("@Nombre", "%" + txtNombre.Text.Trim() + "%");
                    }
                    if (!string.IsNullOrWhiteSpace(txtCategoria.Text))
                    {
                        query += " AND Categoria LIKE @Categoria";
                        cmd.Parameters.AddWithValue("@Categoria", "%" + txtCategoria.Text.Trim() + "%");
                    }
                    if (!string.IsNullOrWhiteSpace(txtPrecio.Text))
                    {
                        query += " AND Precio = @Precio";
                        cmd.Parameters.AddWithValue("@Precio", Convert.ToDecimal(txtPrecio.Text.Trim()));
                    }
                    if (!string.IsNullOrWhiteSpace(txtExistencias.Text))
                    {
                        query += " AND Existencias = @Existencias";
                        cmd.Parameters.AddWithValue("@Existencias", Convert.ToInt32(txtExistencias.Text.Trim()));
                    }

                    cmd.CommandText = query;
                    cmd.Connection = con;

                    try
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            dataGridView1.DataSource = dt;

                            DataRow row = dt.Rows[0];
                            txtIdproducto.Text = row["IdProducto"].ToString();
                            txtNombre.Text = row["Nombre"].ToString();
                            txtCategoria.Text = row["Categoria"].ToString();
                            txtPrecio.Text = row["Precio"].ToString();
                            txtExistencias.Text = row["Existencias"].ToString();

                            MessageBox.Show("¡Consulta realizada con éxito!", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No se encontraron registros con esos criterios de búsqueda.", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al consultar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtIdproducto.Text))
            {
                MessageBox.Show("Seleccione o consulte un producto para actualizar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio <= 0 ||
                !int.TryParse(txtExistencias.Text, out int existencias) || existencias < 0)
            {
                MessageBox.Show("Verifique que los valores numéricos sean correctos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "UPDATE Productos SET Nombre = @Nombre, Categoria = @Categoria, Precio = @Precio, Existencias = @Existencias WHERE IdProducto = @IdProducto";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdProducto", Convert.ToInt32(txtIdproducto.Text));
                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Categoria", txtCategoria.Text.Trim());
                    cmd.Parameters.AddWithValue("@Precio", precio);
                    cmd.Parameters.AddWithValue("@Existencias", existencias);

                    try
                    {
                        con.Open();
                        int filas = cmd.ExecuteNonQuery();
                        if (filas > 0)
                        {
                            MessageBox.Show("¡Producto actualizado correctamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LimpiarCampos();
                            CargarDatos();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocurrió un error al actualizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtIdproducto.Text))
            {
                MessageBox.Show("Seleccione o consulte un producto para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este producto?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                string query = "DELETE FROM Productos WHERE IdProducto = @IdProducto";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@IdProducto", Convert.ToInt32(txtIdproducto.Text));

                        try
                        {
                            con.Open();
                            int filas = cmd.ExecuteNonQuery();
                            if (filas > 0)
                            {
                                MessageBox.Show("¡Producto eliminado correctamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LimpiarCampos();
                                CargarDatos();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ocurrió un error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            CargarDatos();
        }

        private void LimpiarCampos()
        {
            txtIdproducto.Clear();
            txtNombre.Clear();
            txtCategoria.Clear();
            txtPrecio.Clear();
            txtExistencias.Clear();
            txtNombre.Focus();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtIdproducto.Text = row.Cells["IdProducto"].Value.ToString();
                txtNombre.Text = row.Cells["Nombre"].Value.ToString();
                txtCategoria.Text = row.Cells["Categoria"].Value.ToString();
                txtPrecio.Text = row.Cells["Precio"].Value.ToString();
                txtExistencias.Text = row.Cells["Existencias"].Value.ToString();
            }
        }
    }
}