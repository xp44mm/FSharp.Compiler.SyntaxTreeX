module FSharp.Compiler.SyntaxTreeX.Dir

open System.IO

let xp44mm = 
    DirectoryInfo(__SOURCE_DIRECTORY__)
        .Parent.Parent.FullName

let exprPath = Path.Combine(xp44mm,@"FslexFsyacc\FslexFsyacc.Test\Expr")

let fsyacc = Path.Combine(exprPath,"expr.fsyacc")
let parseTable = Path.Combine(exprPath,"ExprParseTable.fs")
