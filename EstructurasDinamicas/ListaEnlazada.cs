using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EstructurasDinamicas
{
    /// <summary>
    /// Lista doblemente enlazada genérica con nodos dinámicos.
    /// Guarda referencias a la cabeza y a la cola, por lo que insertar y
    /// eliminar en ambos extremos es O(1).
    /// </summary>
    /// <typeparam name="T">Tipo de los elementos.</typeparam>
    public class ListaEnlazada<T> : IEnumerable<T>
    {
        private Nodo<T> cabeza;
        private Nodo<T> cola;
        private int tamanio;

        public int Tamanio
        {
            get { return tamanio; }
        }

        public bool EstaVacia
        {
            get { return tamanio == 0; }
        }

        /// <summary>Inserta al inicio. O(1).</summary>
        public void InsertarAlInicio(T dato)
        {
            Nodo<T> nuevo = new Nodo<T>(dato);
            if (EstaVacia)
            {
                cabeza = nuevo;
                cola = nuevo;
            }
            else
            {
                nuevo.Siguiente = cabeza;
                cabeza.Anterior = nuevo;
                cabeza = nuevo;
            }
            tamanio++;
        }

        /// <summary>Inserta al final. O(1) gracias a la referencia a la cola.</summary>
        public void InsertarAlFinal(T dato)
        {
            Nodo<T> nuevo = new Nodo<T>(dato);
            if (EstaVacia)
            {
                cabeza = nuevo;
                cola = nuevo;
            }
            else
            {
                nuevo.Anterior = cola;
                cola.Siguiente = nuevo;
                cola = nuevo;
            }
            tamanio++;
        }

        /// <summary>Inserta en la posición indicada (0 = inicio, Tamanio = final). O(n).</summary>
        public void InsertarEn(int indice, T dato)
        {
            if (indice < 0 || indice > tamanio)
                throw new ArgumentOutOfRangeException("indice", "Índice fuera de rango: " + indice);

            if (indice == 0)
            {
                InsertarAlInicio(dato);
                return;
            }
            if (indice == tamanio)
            {
                InsertarAlFinal(dato);
                return;
            }

            Nodo<T> actual = NodoEn(indice);
            Nodo<T> nuevo = new Nodo<T>(dato);
            nuevo.Anterior = actual.Anterior;
            nuevo.Siguiente = actual;
            actual.Anterior.Siguiente = nuevo;
            actual.Anterior = nuevo;
            tamanio++;
        }

        /// <summary>Elimina y devuelve el primer elemento. O(1).</summary>
        public T EliminarPrimero()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La lista está vacía.");

            T dato = cabeza.Dato;
            cabeza = cabeza.Siguiente;
            if (cabeza == null)
                cola = null;
            else
                cabeza.Anterior = null;
            tamanio--;
            return dato;
        }

        /// <summary>Elimina y devuelve el último elemento. O(1) por ser doblemente enlazada.</summary>
        public T EliminarUltimo()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La lista está vacía.");

            T dato = cola.Dato;
            cola = cola.Anterior;
            if (cola == null)
                cabeza = null;
            else
                cola.Siguiente = null;
            tamanio--;
            return dato;
        }

        /// <summary>Elimina y devuelve el elemento de la posición indicada. O(n).</summary>
        public T EliminarEn(int indice)
        {
            ValidarIndice(indice);

            if (indice == 0)
                return EliminarPrimero();
            if (indice == tamanio - 1)
                return EliminarUltimo();

            Nodo<T> actual = NodoEn(indice);
            actual.Anterior.Siguiente = actual.Siguiente;
            actual.Siguiente.Anterior = actual.Anterior;
            tamanio--;
            return actual.Dato;
        }

        /// <summary>Elimina la primera aparición del dato. Devuelve true si lo encontró. O(n).</summary>
        public bool Eliminar(T dato)
        {
            int i = IndiceDe(dato);
            if (i == -1)
                return false;
            EliminarEn(i);
            return true;
        }

        public T Obtener(int indice)
        {
            ValidarIndice(indice);
            return NodoEn(indice).Dato;
        }

        public T Primero()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La lista está vacía.");
            return cabeza.Dato;
        }

        public T Ultimo()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La lista está vacía.");
            return cola.Dato;
        }

        /// <summary>Devuelve el índice de la primera aparición o -1 si no existe. O(n).</summary>
        public int IndiceDe(T dato)
        {
            Nodo<T> actual = cabeza;
            int i = 0;
            while (actual != null)
            {
                if (EqualityComparer<T>.Default.Equals(actual.Dato, dato))
                    return i;
                actual = actual.Siguiente;
                i++;
            }
            return -1;
        }

        public bool Contiene(T dato)
        {
            return IndiceDe(dato) != -1;
        }

        /// <summary>Invierte la lista cambiando enlaces (sin crear nodos nuevos). O(n).</summary>
        public void Invertir()
        {
            Nodo<T> actual = cabeza;
            Nodo<T> temporal = null;
            while (actual != null)
            {
                temporal = actual.Anterior;
                actual.Anterior = actual.Siguiente;
                actual.Siguiente = temporal;
                actual = actual.Anterior;
            }
            temporal = cabeza;
            cabeza = cola;
            cola = temporal;
        }

        public void Vaciar()
        {
            cabeza = null;
            cola = null;
            tamanio = 0;
        }

        // ---------- Auxiliares ----------

        private void ValidarIndice(int indice)
        {
            if (indice < 0 || indice >= tamanio)
                throw new ArgumentOutOfRangeException("indice", "Índice fuera de rango: " + indice);
        }

        /// <summary>Recorre desde el extremo más cercano al índice pedido.</summary>
        private Nodo<T> NodoEn(int indice)
        {
            Nodo<T> actual;
            if (indice < tamanio / 2)
            {
                actual = cabeza;
                for (int i = 0; i < indice; i++)
                    actual = actual.Siguiente;
            }
            else
            {
                actual = cola;
                for (int i = tamanio - 1; i > indice; i--)
                    actual = actual.Anterior;
            }
            return actual;
        }

        // ---------- IEnumerable<T> (permite usar foreach) ----------

        public IEnumerator<T> GetEnumerator()
        {
            Nodo<T> actual = cabeza;
            while (actual != null)
            {
                yield return actual.Dato;
                actual = actual.Siguiente;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("[");
            Nodo<T> actual = cabeza;
            while (actual != null)
            {
                sb.Append(actual.Dato);
                if (actual.Siguiente != null)
                    sb.Append(" <-> ");
                actual = actual.Siguiente;
            }
            return sb.Append("]").ToString();
        }
    }
}
