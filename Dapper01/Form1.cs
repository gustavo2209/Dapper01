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
using Dapper;

namespace Dapper01
{
    public partial class Form1 : Form
    {

        private string data_connection;

        public Form1()
        {
            InitializeComponent();
            data_connection = "Data Source=(local);Initial Catalog=parainfo;Integrated Security=SSPI";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = "SELECT idalumno, nombre FROM alumnos2";

            using(var cn = new SqlConnection(data_connection))
            {
                var lista = cn.Query<Alumnos>(sql).ToList<Alumnos>();

                string text = "";

                foreach(Alumnos a in lista)
                {
                    text += a.Idalumno + " " + a.Nombre + "\r\n";
                }

                textBox1.Text = text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // CONSULTA CON LEFT JOIN

            string sql = "SELECT alumnos2.nombre AS 'Alumno', notas.nota AS 'Nota' FROM " +
                "alumnos2 LEFT JOIN notas " +
                "ON alumnos2.idalumno = notas.idalumno";

            using (var cn = new SqlConnection(data_connection))
            {
                dynamic lista = cn.Query(sql).ToList();

                string text = "";

                for(int i = 0; i < lista.Count; i++)
                {
                    text += lista[i].Alumno + " " + lista[i].Nota + "\r\n";
                }

                textBox1.Text = text;
            }


            // CONSULTA CON PARAMETROS

            string sql2 = "SELECT nota AS 'Nota' FROM " +
                "notas " +
                "WHERE idalumno = @idalumno";

            using (var cn = new SqlConnection(data_connection))
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary.Add("@idalumno", 1);

                dynamic lista = cn.Query(sql2, new DynamicParameters(dictionary)).ToList();

                string text = "Notas de Anabel Aranda: \r\n";

                for (int i = 0; i < lista.Count; i++)
                {
                    text += "\t\t\t" + lista[i].Nota + "\r\n";
                }

                textBox1.Text = text;
            }

            // CONSULTAS MULTIPLES

            string sql3 = "SELECT * FROM Alumnos WHERE idalumno = @idalumno;" +
                "SELECT * FROM notas WHERE idalumno = @idalumno";

            using (var cn = new SqlConnection(data_connection))
            {
               using(var multi = cn.QueryMultiple(sql3, new {idalumno = 1 }))
                {
                    var alumno = multi.Read<Alumnos>().First();
                    var notas = multi.Read<Notas>().ToList();

                    string text = "Notas de " + alumno.Nombre + "\r\n";

                    foreach(Notas n in notas)
                    {
                        text += "\t\t\t[" + n.Idnota + "] " + n.Nota + "\r\n";
                    }

                    textBox1.Text = text;
                }
            }

            // CONSULTA DE ÚNICA FILA

            string sql4 = "SELECT * FROM alumnos WHERE idalumno = @idalumno";

            using (var cn = new SqlConnection(data_connection))
            {
                var alumno = cn.QuerySingle<Alumnos>(sql4, new {idalumno = 2 });

                string text = "ID: " + alumno.Idalumno + ", alumno: " + alumno.Nombre;

                textBox1.Text = text;
            }

            // CONSULTA CON CARACTERES COMODINES

            //string sql5 = "SELECT * FROM alumnos WHERE nombre LIKE 'J%'";
            string sql5 = "SELECT * FROM alumnos WHERE nombre LIKE '%a%'";

            using (var cn = new SqlConnection(data_connection))
            {
                dynamic lista = cn.Query<Alumnos>(sql5).ToList<Alumnos>();

                string text = "";

                foreach(Alumnos a in lista)
                {
                    text += "[" + a.Idalumno + "] " + a.Nombre + "\r\n";
                }

                textBox1.Text = text;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sql01 = "INSERT alumnos2(nombre) OUTPUT inserted.idalumno VALUES(@nombre)";
            string sql02 = "INSERT notas(idalumno, nota) VALUES(@idalumno, @nota)";

            using (var cn = new SqlConnection(data_connection))
            {
                var id = cn.QuerySingle<int>(sql01, new { nombre = "Alumno de Prueba" });

                for(int i = 0; i < 3; i++)
                {
                    cn.Execute(sql02, new { idalumno = id, nota = 20});
                }

                textBox1.Text = "INSERCION EXITOSA";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sql = "DELETE FROM alumnos2 WHERE idalumno = @idalumno";

            using (var cn = new SqlConnection(data_connection))
            {
                cn.Execute(sql, new { idalumno = 2002 });
            }

            textBox1.Text = "RETIRO EXITOSO";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string sql = "UPDATE notas SET nota = @nota1 WHERE idalumno = @idalumno AND nota = @nota2";

            using (var cn = new SqlConnection(data_connection))
            {
                cn.Execute(sql, new { nota1 = 20, idalumno = 1, nota2 = 15 });
            }

            textBox1.Text = "ACTUALIZACIÓN EXITOSA";
        }
    }
}
