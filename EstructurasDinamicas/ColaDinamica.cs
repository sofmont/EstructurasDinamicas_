using System;
using System.Collections;
using System.Collections.Generic;

namespace EstructurasDinamicas
{
    /// <summary>
    /// Cola (FIFO) genérica dinámica. Se encola al final y se desencola por el
    /// frente; ambas operaciones son O(1) y la cola nunca se "llena".
    /// </summary>
    /// <typeparam name="T">Tipo de los elementos.</typeparam>
    public class ColaDinamica<T> : IEnumerable<T>
    {
        private readonly ListaEnlazada<T> lista = new ListaEnlazada<T>();

        public int Tamanio
        {
            get { return lista.Tamanio; }
        }

        public bool EstaVacia
        {
            get { return lista.EstaVacia; }
        }

        /// <summary>Agrega un elemento al final de la cola.</summary>
        public void Encolar(T dato)
        {
            lista.InsertarAlFinal(dato);
        }

        /// <summary>Quita y devuelve el elemento del frente.</summary>
        public T Desencolar()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La cola está vacía (underflow).");
            return lista.EliminarPrimero();
        }

        /// <summary>Consulta el frente sin quitarlo.</summary>
        public T Frente()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La cola está vacía.");
            return lista.Primero();
        }

        public void Vaciar()
        {
            lista.Vaciar();
        }

        /// <summary>Recorre del frente hacia el final.</summary>
        public IEnumerator<T> GetEnumerator()
        {
            return lista.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return "Cola (frente -> final) " + lista;
        }
    }
}
