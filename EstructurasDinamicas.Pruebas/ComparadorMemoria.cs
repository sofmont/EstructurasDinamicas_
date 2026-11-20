using EstructurasDinamicas;

namespace EstructurasDinamicas.Pruebas;

/// <summary>Resultado de medir la memoria de una estructura.</summary>
public record MedicionMemoria(string Estructura, int Elementos, long Bytes)
{
    public string BytesPorElemento => Elementos == 0 ? "—" : (Bytes / (double)Elementos).ToString("0.##");
}

/// <summary>
/// Mide la memoria que realmente reserva cada estructura en .NET para comparar
/// el desperdicio de memoria (estático) con la sobrecarga de punteros (dinámico).
/// </summary>
public static class ComparadorMemoria
{
    // Nodo de lista simple, solo para comparar su tamaño con el nodo doble.
    private sealed class NodoSimple
    {
        public string Dato;
        public NodoSimple Siguiente;
    }

    public static List<MedicionMemoria> Medir(int elementos)
    {
        if (elementos < 1) throw new ArgumentOutOfRangeException(nameof(elementos), "Debe haber al menos un elemento.");
        const string dato = "x";   // todas guardan la misma referencia: solo se mide la estructura

        return new List<MedicionMemoria>
        {
            new($"Arreglo estático (capacidad {elementos})", elementos,
                BytesReservados(() => GC.KeepAlive(new string[elementos]))),
            new("ListaEnlazada<T> propia (nodo doble)", elementos, BytesReservados(() =>
            {
                var lista = new ListaEnlazada<string>();
                for (int i = 0; i < elementos; i++) lista.InsertarAlFinal(dato);
                GC.KeepAlive(lista);
            }) - BytesReservados(() => GC.KeepAlive(new ListaEnlazada<string>()))),
            new("Lista simple (sin enlace Anterior)", elementos, BytesReservados(() =>
            {
                NodoSimple cabeza = null;
                for (int i = 0; i < elementos; i++) cabeza = new NodoSimple { Dato = dato, Siguiente = cabeza };
                GC.KeepAlive(cabeza);
            })),
            new("List<T> de .NET (arreglo que se duplica)", elementos, BytesReservados(() =>
            {
                var lista = new List<string>();
                for (int i = 0; i < elementos; i++) lista.Add(dato);
                GC.KeepAlive(lista);
            }) - BytesReservados(() => GC.KeepAlive(new List<string>()))),
            new("LinkedList<T> de .NET", elementos, BytesReservados(() =>
            {
                var lista = new LinkedList<string>();
                for (int i = 0; i < elementos; i++) lista.AddLast(dato);
                GC.KeepAlive(lista);
            }) - BytesReservados(() => GC.KeepAlive(new LinkedList<string>()))),
        };
    }

    // Bytes asignados por el hilo actual al ejecutar la acción (se ejecuta una vez antes para descartar el JIT).
    private static long BytesReservados(Action accion)
    {
        accion();
        long antes = GC.GetAllocatedBytesForCurrentThread();
        accion();
        return GC.GetAllocatedBytesForCurrentThread() - antes;
    }
}
