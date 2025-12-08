using System;
using Wasmtime;

using var engine = new Engine();

using var module = Module.FromText(
    engine,
    "hello",
    "(module (func $hello (import \"\" \"hello\")) (func (export \"run\") (call $hello)))"
);

using var linker = new Linker(engine);
using var store = new Store(engine);

linker.Define(
    "",
    "hello",
    Function.FromCallback(store, () => Console.WriteLine("Hello from C#!"))
);

var instance = linker.Instantiate(store, module);
var run = instance.GetAction("run")!;
run();


// using System;
// using Wasmtime;

// using var engine = new Engine();
// using var module = Module.FromTextFile(engine, "global.wat");
// using var linker = new Linker(engine);
// using var store = new Store(engine);

// var global = new Global(store, ValueKind.Int32, 1, Mutability.Mutable);

// linker.Define("", "global", global);

// linker.Define(
//     "",
//     "print_global",
//     Function.FromCallback(store, (Caller caller) =>
//     {
//         Console.WriteLine($"The value of the global is: {global.GetValue()}.");
//     }
// ));

// var instance = linker.Instantiate(store, module);

// var run = instance.GetAction<int>("run");
// if (run is null)
// {
//     Console.WriteLine("error: run export is missing");
//     return;
// }

// run(20);