using System;
using System.Collections;
using System.Collections.Generic;

namespace EstructurasDinamicas
{
    /// <summary>
    /// Pila (LIFO) genérica dinámica. El tope es la cabeza de la lista enlazada,
    /// así que Push, Pop y Peek son O(1) y no existe "pila llena".
    /// </summary>
    /// <typeparam name="T">Tipo de los elementos.</typeparam>
    public class PilaDinamica<T> : IEnumerable<T>
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

        /// <summary>Apila un elemento en el tope.</summary>
        public void Push(T dato)
        {
            lista.InsertarAlInicio(dato);
        }

        /// <summary>Desapila y devuelve el elemento del tope.</summary>
        public T Pop()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La pila está vacía (underflow).");
            return lista.EliminarPrimero();
        }

        /// <summary>Consulta el tope sin quitarlo.</summary>
        public T Peek()
        {
            if (EstaVacia)
                throw new InvalidOperationException("La pila está vacía.");
            return lista.Primero();
        }

        public void Vaciar()
        {
            lista.Vaciar();
        }

        /// <summary>Recorre del tope hacia la base.</summary>
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
            return "Pila (tope -> base) " + lista;
        }
    }
}
