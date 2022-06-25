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
    member _.``valid ParseTable``() =
        let fsyaccParseTableFile = 
            let text = File.ReadAllText(Dir.fsyacc, Encoding.UTF8)
            let rawFsyacc = FsyaccFile.parse text
            let fsyacc = NormFsyaccFile.fromRaw rawFsyacc           
            fsyacc.toFsyaccParseTableFile()

        let parseTableDecls =
            let text = File.ReadAllText(Dir.parseTable, Encoding.UTF8)
            Parser.getDecls("parsetable.fs",text)

        let headerFromFsyacc =
            let decls = 
                FSharp.Compiler.SyntaxTreeX.Parser.getDecls("header.fsx",fsyaccParseTableFile.header)
            decls

        let semansFsyacc =
            let mappers = fsyaccParseTableFile.generateMappers()
            FSharp.Compiler.SyntaxTreeX.SourceCodeParser.semansFromFsyacc mappers

        let header,semans =
            FSharp.Compiler.SyntaxTreeX.SourceCodeParser.fromParseTable parseTableDecls

        Should.equal headerFromFsyacc header
        Should.equal semansFsyacc semans

