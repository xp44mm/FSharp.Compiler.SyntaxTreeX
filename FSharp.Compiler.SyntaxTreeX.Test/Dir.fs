module FSharp.Compiler.SyntaxTreeX.Dir

open System.IO

let xp44mm = 
    DirectoryInfo(__SOURCE_DIRECTORY__)
        .Parent.Parent.FullName

let testPath = Path.Combine(xp44mm,@"FslexFsyacc\FslexFsyacc.Test")
let exprPath = Path.Combine(testPath,@"Expr")
let fsyacc = Path.Combine(exprPath,"expr.fsyacc")
let parseTable = Path.Combine(exprPath,"ExprParseTable.fs")

let termPath = Path.Combine(testPath,@"PolynomialExpressions")
let fslex = Path.Combine(termPath,"term.fslex")
let dfa = Path.Combine(termPath,"TermDFA.fs")
