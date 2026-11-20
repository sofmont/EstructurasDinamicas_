namespace EstructurasDinamicas
{
    /// <summary>
    /// Nodo genérico para estructuras dinámicas doblemente enlazadas.
    /// Cada nodo se crea con "new" en el heap solo cuando se necesita.
    /// </summary>
    /// <typeparam name="T">Tipo del dato almacenado.</typeparam>
    public class Nodo<T>
    {
        public T Dato;
        public Nodo<T> Siguiente;
        public Nodo<T> Anterior;

        public Nodo(T dato)
        {
            Dato = dato;
            Siguiente = null;
            Anterior = null;
        }
    }
}
