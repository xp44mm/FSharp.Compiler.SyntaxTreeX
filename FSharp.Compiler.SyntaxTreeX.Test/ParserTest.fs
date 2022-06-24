namespace FSharp.Compiler.SyntaxTreeX

open Xunit
open Xunit.Abstractions

open System.IO
open System.Text

open FSharp.Literals
open FSharp.xUnit

open FslexFsyacc.Fsyacc


type ParserTest(output: ITestOutputHelper) =
    let show res =
        res |> Render.stringify |> output.WriteLine

    [<Fact>]
    member _.``1 - header equality``() =
        let src =
            let text = File.ReadAllText(Dir.fsyacc, Encoding.UTF8)
            let rawFsyacc = FsyaccFile.parse text
            let header = rawFsyacc.header
            let decls:XModuleDecl list = 
                Parser.getDecls("header.fsx",header)
            decls

        let len = src.Length

        let tgt =
            let text = File.ReadAllText(Dir.parseTable, Encoding.UTF8)
            SourceCodeParser.headerFromParseTable text len

        Should.equal src tgt

    [<Fact>]
    member _.``2 - semans equality``() =
        let semansFsyacc =
            let text = File.ReadAllText(Dir.fsyacc, Encoding.UTF8)
            let rawFsyacc = FsyaccFile.parse text
            let fsyacc = NormFsyaccFile.fromRaw rawFsyacc
            let parseTbl = fsyacc.toFsyaccParseTableFile()
            let mappers = parseTbl.generateMappers()
            SourceCodeParser.semansFromFsyacc mappers

        let semansParseTable =
            let text = File.ReadAllText(Dir.parseTable, Encoding.UTF8)
            SourceCodeParser.semansFromParseTable text

        Should.equal semansFsyacc semansParseTable
