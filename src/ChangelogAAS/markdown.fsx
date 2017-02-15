module MarkdownToHtml

#r "CommonMark"
#r "System.Net.Http"

open CommonMark
open System.Net
open System.Net.Http
open System.Text

let markdownAsHtmlPageResponse markdownString =
    let body = CommonMark.CommonMarkConverter.Convert(markdownString);
    let site = sprintf """
        <!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
        <html lang="en">
        <head>
            <meta http-equiv="content-type" content="text/html; charset=utf-8">
            <title>Changelog</title>
            <!-- Bootstrap core CSS -->
            <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" rel="stylesheet">
        </head>
        <body>
            <div class="container">
                %s
            </div>
        </body>
        </html>""" body
    let bytes = Encoding.Default.GetBytes(site);
    new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(Encoding.UTF8.GetString(bytes),Encoding.UTF8,"text/html"))