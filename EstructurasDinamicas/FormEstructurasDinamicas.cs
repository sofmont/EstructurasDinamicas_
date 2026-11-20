using System;
using System.Drawing;
using System.Windows.Forms;

namespace EstructurasDinamicas
{
    /// <summary>
    /// Formulario de demostración (sin diseñador): tres pestañas para probar
    /// la lista enlazada, la pila dinámica y la cola dinámica.
    /// Para usarlo: Application.Run(new FormEstructurasDinamicas());
    /// o abrirlo desde otro formulario con new FormEstructurasDinamicas().Show();
    /// </summary>
    public class FormEstructurasDinamicas : Form
    {
        private readonly ListaEnlazada<string> lista = new ListaEnlazada<string>();
        private readonly PilaDinamica<string> pila = new PilaDinamica<string>();
        private readonly ColaDinamica<string> cola = new ColaDinamica<string>();

        private TextBox txtLista, txtIndice, txtPila, txtCola;
        private ListBox lbLista, lbPila, lbCola;
        private Label lblLista, lblPila, lblCola;

        public FormEstructurasDinamicas()
        {
            Text = "Tarea 3.2 - Estructuras dinámicas";
            Size = new Size(820, 520);
            StartPosition = FormStartPosition.CenterScreen;

            txtLista = new TextBox { Width = 130 };
            txtIndice = new TextBox { Width = 40 };
            txtPila = new TextBox { Width = 130 };
            txtCola = new TextBox { Width = 130 };
            lbLista = CrearListBox();
            lbPila = CrearListBox();
            lbCola = CrearListBox();
            lblLista = CrearEstado();
            lblPila = CrearEstado();
            lblCola = CrearEstado();

            TabControl tabs = new TabControl { Dock = DockStyle.Fill };

            tabs.TabPages.Add(ArmarPestana("Lista enlazada", lbLista, lblLista, new Control[]
            {
                Etiqueta("Dato:"), txtLista,
                Etiqueta("Índice:"), txtIndice,
                Boton("Insertar al inicio", (s, e) => AccionLista(() => lista.InsertarAlInicio(DatoLista()))),
                Boton("Insertar al final", (s, e) => AccionLista(() => lista.InsertarAlFinal(DatoLista()))),
                Boton("Insertar en índice", (s, e) => AccionLista(() => lista.InsertarEn(IndiceLista(), DatoLista()))),
                Boton("Eliminar primero", (s, e) => AccionLista(() => lista.EliminarPrimero())),
                Boton("Eliminar último", (s, e) => AccionLista(() => lista.EliminarUltimo())),
                Boton("Eliminar en índice", (s, e) => AccionLista(() => lista.EliminarEn(IndiceLista()))),
                Boton("Buscar dato", (s, e) => BuscarEnLista()),
                Boton("Invertir", (s, e) => AccionLista(() => lista.Invertir())),
                Boton("Vaciar", (s, e) => AccionLista(() => lista.Vaciar()))
            }));

            tabs.TabPages.Add(ArmarPestana("Pila (LIFO)", lbPila, lblPila, new Control[]
            {
                Etiqueta("Dato:"), txtPila,
                Boton("Push", (s, e) => AccionPila(() => pila.Push(Requerido(txtPila)), null)),
                Boton("Pop", (s, e) => AccionPila(null, () => "Pop: " + pila.Pop())),
                Boton("Peek", (s, e) => AccionPila(null, () => "Tope: " + pila.Peek())),
                Boton("Vaciar", (s, e) => AccionPila(() => pila.Vaciar(), null))
            }));

            tabs.TabPages.Add(ArmarPestana("Cola (FIFO)", lbCola, lblCola, new Control[]
            {
                Etiqueta("Dato:"), txtCola,
                Boton("Encolar", (s, e) => AccionCola(() => cola.Encolar(Requerido(txtCola)), null)),
                Boton("Desencolar", (s, e) => AccionCola(null, () => "Desencolado: " + cola.Desencolar())),
                Boton("Frente", (s, e) => AccionCola(null, () => "Frente: " + cola.Frente())),
                Boton("Vaciar", (s, e) => AccionCola(() => cola.Vaciar(), null))
            }));

            Controls.Add(tabs);
            RefrescarTodo();
        }

        // ---------- Construcción de la interfaz ----------

        private static ListBox CrearListBox()
        {
            return new ListBox { Dock = DockStyle.Fill, Font = new Font("Consolas", 11f) };
        }

        private static Label CrearEstado()
        {
            return new Label { Dock = DockStyle.Bottom, Height = 28, TextAlign = ContentAlignment.MiddleLeft };
        }

        private static Label Etiqueta(string texto)
        {
            return new Label { Text = texto, AutoSize = true, Margin = new Padding(6, 9, 2, 0) };
        }

        private static Button Boton(string texto, EventHandler accion)
        {
            Button b = new Button { Text = texto, AutoSize = true };
            b.Click += accion;
            return b;
        }

        private static TabPage ArmarPestana(string titulo, ListBox lb, Label estado, Control[] controles)
        {
            TabPage tab = new TabPage(titulo);
            // AutoSize: la barra crece si los botones no caben (por ejemplo, con la pantalla al 125 % o 150 %).
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(6),
                WrapContents = true
            };
            panel.Controls.AddRange(controles);

            // Se agrega primero el control que ocupa el resto (Fill).
            tab.Controls.Add(lb);
            tab.Controls.Add(estado);
            tab.Controls.Add(panel);
            return tab;
        }

        // ---------- Lectura de datos de entrada ----------

        private static string Requerido(TextBox caja)
        {
            string texto = caja.Text.Trim();
            if (texto.Length == 0)
                throw new InvalidOperationException("Escribe un dato primero.");
            return texto;
        }

        private string DatoLista()
        {
            return Requerido(txtLista);
        }

        private int IndiceLista()
        {
            int indice;
            if (!int.TryParse(txtIndice.Text, out indice))
                throw new InvalidOperationException("El índice debe ser un número entero.");
            return indice;
        }

        // ---------- Acciones (capturan errores y refrescan la vista) ----------

        private void AccionLista(Action accion)
        {
            try
            {
                accion();
                txtLista.Clear();
                RefrescarLista();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BuscarEnLista()
        {
            try
            {
                string dato = DatoLista();
                int i = lista.IndiceDe(dato);
                lblLista.Text = i == -1
                    ? "\"" + dato + "\" no está en la lista."
                    : "\"" + dato + "\" está en el índice " + i + ".";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AccionPila(Action modifica, Func<string> consulta)
        {
            try
            {
                string mensaje = null;
                if (modifica != null) modifica();
                if (consulta != null) mensaje = consulta();
                txtPila.Clear();
                RefrescarPila();
                if (mensaje != null) lblPila.Text = mensaje + "   |   " + lblPila.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AccionCola(Action modifica, Func<string> consulta)
        {
            try
            {
                string mensaje = null;
                if (modifica != null) modifica();
                if (consulta != null) mensaje = consulta();
                txtCola.Clear();
                RefrescarCola();
                if (mensaje != null) lblCola.Text = mensaje + "   |   " + lblCola.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ---------- Refrescar la vista ----------

        private void RefrescarTodo()
        {
            RefrescarLista();
            RefrescarPila();
            RefrescarCola();
        }

        private void RefrescarLista()
        {
            lbLista.Items.Clear();
            int i = 0;
            foreach (string dato in lista)
            {
                lbLista.Items.Add("[" + i + "] " + dato);
                i++;
            }
            lblLista.Text = "Tamaño: " + lista.Tamanio + "   " + lista;
        }

        private void RefrescarPila()
        {
            lbPila.Items.Clear();
            bool primero = true;
            foreach (string dato in pila)
            {
                lbPila.Items.Add(primero ? "TOPE -> " + dato : "         " + dato);
                primero = false;
            }
            lblPila.Text = "Tamaño: " + pila.Tamanio;
        }

        private void RefrescarCola()
        {
            lbCola.Items.Clear();
            int i = 0;
            foreach (string dato in cola)
            {
                string marca = i == 0 ? "FRENTE -> " : "           ";
                lbCola.Items.Add(marca + dato);
                i++;
            }
            lblCola.Text = "Tamaño: " + cola.Tamanio;
        }
    }
}
