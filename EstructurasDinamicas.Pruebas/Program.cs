using EstructurasDinamicas;
using EstructurasDinamicas.Pruebas;

// Pruebas automáticas de ListaEnlazada, PilaDinamica y ColaDinamica. No usa bibliotecas externas:
// cada prueba devuelve true si el resultado es el esperado.
// Ejecutar: establecer este proyecto como proyecto de inicio y presionar Ctrl+F5.

int aprobadas = 0, fallidas = 0;

void Seccion(string titulo)
{
    Console.WriteLine();
    Console.WriteLine(titulo);
    Console.WriteLine(new string('-', 76));
}

void Probar(string id, string descripcion, Func<bool> prueba)
{
    bool ok;
    try { ok = prueba(); }
    catch (Exception ex) { ok = false; descripcion += $"  [excepción: {ex.Message}]"; }
    if (ok) aprobadas++; else fallidas++;
    Console.WriteLine($"{(ok ? "[OK]   " : "[FALLA]")} {id,-5} {descripcion}");
}

bool Lanza<TExcepcion>(Action accion) where TExcepcion : Exception
{
    try { accion(); return false; }
    catch (TExcepcion) { return true; }
}

Console.WriteLine("PRUEBAS DE ESTRUCTURAS DINÁMICAS");
Console.WriteLine(new string('=', 76));

// ------------------------------------------------------------------ Lista enlazada
Seccion("ListaEnlazada<T> (doblemente enlazada)");

Probar("L-01", "InsertarAlFinal A, B, C: recorrido A, B, C", () =>
{
    var l = new ListaEnlazada<string>();
    l.InsertarAlFinal("A"); l.InsertarAlFinal("B"); l.InsertarAlFinal("C");
    return string.Join(",", l) == "A,B,C" && l.Tamanio == 3;
});
Probar("L-02", "InsertarAlInicio C, B, A: recorrido A, B, C", () =>
{
    var l = new ListaEnlazada<string>();
    l.InsertarAlInicio("C"); l.InsertarAlInicio("B"); l.InsertarAlInicio("A");
    return string.Join(",", l) == "A,B,C" && l.Primero() == "A" && l.Ultimo() == "C";
});
Probar("L-03", "InsertarEn(1, X) en [A, B, C]: [A, X, B, C]", () =>
{
    var l = new ListaEnlazada<string>();
    l.InsertarAlFinal("A"); l.InsertarAlFinal("B"); l.InsertarAlFinal("C");
    l.InsertarEn(1, "X");
    return l.ToString() == "[A <-> X <-> B <-> C]";
});
Probar("L-04", "EliminarPrimero y EliminarUltimo devuelven los extremos", () =>
{
    var l = new ListaEnlazada<int>();
    foreach (int x in new[] { 1, 2, 3, 4 }) l.InsertarAlFinal(x);
    return l.EliminarPrimero() == 1 && l.EliminarUltimo() == 4 && string.Join(",", l) == "2,3";
});
Probar("L-05", "EliminarEn(2) en [10, 20, 30, 40, 50]: devuelve 30", () =>
{
    var l = new ListaEnlazada<int>();
    foreach (int x in new[] { 10, 20, 30, 40, 50 }) l.InsertarAlFinal(x);
    return l.EliminarEn(2) == 30 && string.Join(",", l) == "10,20,40,50";
});
Probar("L-06", "IndiceDe: devuelve el índice o -1 si no existe", () =>
{
    var l = new ListaEnlazada<string>();
    l.InsertarAlFinal("ana"); l.InsertarAlFinal("luis");
    return l.IndiceDe("luis") == 1 && l.IndiceDe("pedro") == -1;
});
Probar("L-07", "Invertir [1, 2, 3, 4]: [4, 3, 2, 1] y los extremos se intercambian", () =>
{
    var l = new ListaEnlazada<int>();
    foreach (int x in new[] { 1, 2, 3, 4 }) l.InsertarAlFinal(x);
    l.Invertir();
    bool enlacesBien = l.EliminarUltimo() == 1 && l.EliminarPrimero() == 4;   // comprueba Anterior y Siguiente
    return enlacesBien && string.Join(",", l) == "3,2";
});
Probar("L-08", "Eliminar el único nodo deja la lista vacía (cabeza y cola en null)", () =>
{
    var l = new ListaEnlazada<int>();
    l.InsertarAlFinal(7);
    l.EliminarUltimo();
    l.InsertarAlInicio(8);   // si cola no quedó en null, este caso fallaría
    return l.Tamanio == 1 && l.Primero() == 8 && l.Ultimo() == 8;
});
Probar("L-09", "Eliminar en lista vacía lanza InvalidOperationException", () =>
    Lanza<InvalidOperationException>(() => new ListaEnlazada<int>().EliminarPrimero()));
Probar("L-10", "Índice fuera de rango lanza ArgumentOutOfRangeException", () =>
{
    var l = new ListaEnlazada<int>();
    l.InsertarAlFinal(1);
    return Lanza<ArgumentOutOfRangeException>(() => l.InsertarEn(5, 2))
        && Lanza<ArgumentOutOfRangeException>(() => l.Obtener(-1));
});

// ------------------------------------------------------------------ Pila
Seccion("PilaDinamica<T> (LIFO)");

Probar("P-01", "Push 1, 2, 3 y tres Pop: sale 3, 2, 1", () =>
{
    var p = new PilaDinamica<int>();
    p.Push(1); p.Push(2); p.Push(3);
    return p.Pop() == 3 && p.Pop() == 2 && p.Pop() == 1 && p.EstaVacia;
});
Probar("P-02", "Peek devuelve el tope sin retirarlo", () =>
{
    var p = new PilaDinamica<string>();
    p.Push("a"); p.Push("b");
    return p.Peek() == "b" && p.Tamanio == 2;
});
Probar("P-03", "Pop con la pila vacía lanza InvalidOperationException", () =>
    Lanza<InvalidOperationException>(() => new PilaDinamica<int>().Pop()));
Probar("P-04", "10 000 Push: la pila dinámica nunca se llena", () =>
{
    var p = new PilaDinamica<int>();
    for (int i = 0; i < 10_000; i++) p.Push(i);
    return p.Tamanio == 10_000 && p.Peek() == 9_999;
});

// ------------------------------------------------------------------ Cola
Seccion("ColaDinamica<T> (FIFO)");

Probar("C-01", "Encolar A, B, C y tres Desencolar: sale A, B, C", () =>
{
    var c = new ColaDinamica<string>();
    c.Encolar("A"); c.Encolar("B"); c.Encolar("C");
    return c.Desencolar() == "A" && c.Desencolar() == "B" && c.Desencolar() == "C" && c.EstaVacia;
});
Probar("C-02", "Frente devuelve el primero sin retirarlo", () =>
{
    var c = new ColaDinamica<int>();
    c.Encolar(5); c.Encolar(6);
    return c.Frente() == 5 && c.Tamanio == 2;
});
Probar("C-03", "Desencolar con la cola vacía lanza InvalidOperationException", () =>
    Lanza<InvalidOperationException>(() => new ColaDinamica<int>().Desencolar()));

// ------------------------------------------------------------------ Clases de .NET
Seccion("Comparación con las clases genéricas de .NET");

Probar("N-01", "PilaDinamica y Stack<T> devuelven el mismo orden", () =>
{
    var propia = new PilaDinamica<int>(); var net = new Stack<int>();
    foreach (int x in new[] { 4, 8, 15 }) { propia.Push(x); net.Push(x); }
    return string.Join(",", propia) == string.Join(",", net);
});
Probar("N-02", "ColaDinamica y Queue<T> devuelven el mismo orden", () =>
{
    var propia = new ColaDinamica<int>(); var net = new Queue<int>();
    foreach (int x in new[] { 4, 8, 15 }) { propia.Encolar(x); net.Enqueue(x); }
    return string.Join(",", propia) == string.Join(",", net);
});
Probar("N-03", "ListaEnlazada y LinkedList<T> coinciden tras insertar al inicio y al final", () =>
{
    var propia = new ListaEnlazada<int>(); var net = new LinkedList<int>();
    propia.InsertarAlFinal(2); net.AddLast(2);
    propia.InsertarAlInicio(1); net.AddFirst(1);
    propia.InsertarAlFinal(3); net.AddLast(3);
    return string.Join(",", propia) == string.Join(",", net);
});
Probar("N-04", "Memoria: arreglo de 1 000 = 8 024 bytes; nodo doble = 40 bytes", () =>
{
    var m = ComparadorMemoria.Medir(1000);
    return m[0].Bytes == 8024 && m[1].Bytes == 40_000 && m[2].Bytes == 32_000;
});

Seccion("Memoria medida con 1 000 elementos (desperdicio de memoria vs. sobrecarga de punteros)");
foreach (MedicionMemoria m in ComparadorMemoria.Medir(1000))
    Console.WriteLine($"  {m.Estructura,-44} {m.Bytes,8:N0} bytes   {m.BytesPorElemento,6} por elemento");

Console.WriteLine();
Console.WriteLine(new string('=', 76));
Console.WriteLine($"Aprobadas: {aprobadas}   Fallidas: {fallidas}   Total: {aprobadas + fallidas}");
return fallidas == 0 ? 0 : 1;
