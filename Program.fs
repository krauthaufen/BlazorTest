open Aardvark.Blazor.Core
open Aardvark.Blazor.WebGL
open Aardvark.Base
open FSharp.Data.Adaptive
open Aardvark.Rendering
open Aardvark.SceneGraph
open Aardvark.Application

let run() =
    async {
        do! JS.Window.Document.Ready
        let e = JS.Window.Document.CreateElement "h1"
        e.InnerHTML <- "AARDVARK ROCKS"
        Window.Document.Body.AppendChild e

        let c = JS.Window.Document.CreateCanvasElement()
        c.Id <- "sepp"
        c.Style.Width <- "50%"
        c.Style.Height <- "50%"
        JS.Window.Document.Body.AppendChild c
        let ctrl = new RenderControl(c)

        let view =
            CameraView.lookAt (V3d(3,4,5)) V3d.Zero V3d.OOI
            |> DefaultCameraController.control ctrl.Mouse ctrl.Keyboard ctrl.Time

        let proj =
            ctrl.Sizes |> AVal.map (fun s -> 
                Frustum.perspective 60.0 0.1 100.0 (float s.X / float s.Y)
            )

        let task =
            Sg.box' C4b.Red Box3d.Unit
            |> Sg.shader {
                do! DefaultSurfaces.trafo
                do! DefaultSurfaces.constantColor C4f.White
                do! DefaultSurfaces.simpleLighting
            }
            |> Sg.viewTrafo (AVal.map CameraView.viewTrafo view)
            |> Sg.projTrafo (AVal.map Frustum.projTrafo proj)
            |> Sg.compile ctrl.Runtime ctrl.FramebufferSignature


        ctrl.RenderTask <- task
        ctrl.Start()

    }

[<EntryPoint>]
let main args =
    Logging.init()
    run() |> Async.Start
    0