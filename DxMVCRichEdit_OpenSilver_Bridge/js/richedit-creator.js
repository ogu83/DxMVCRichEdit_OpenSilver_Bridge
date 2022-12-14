var spellCheckerWorker = null;
var spellCheckerCallbacks = Object.create(null);
var spellCheckerWorkerCommandId = 0;

function createRichEdit(exportUrl) {
    const options = DevExpress.RichEdit.createOptions();

    options.bookmarks.visibility = true;
    options.bookmarks.color = '#ff0000';

    options.confirmOnLosingChanges.enabled = true;
    options.confirmOnLosingChanges.message = 'Are you sure you want to perform the action? All unsaved document data will be lost.';

    options.fields.updateFieldsBeforePrint = true;
    options.fields.updateFieldsOnPaste = true;

    options.mailMerge.activeRecord = 2;
    options.mailMerge.viewMergedData = true;
    options.mailMerge.dataSource = [
        { Name: 'Indy', age: 32 },
        { Name: 'Andy', age: 28 },
    ];

    // events
    options.events.activeSubDocumentChanged = () => { };
    options.events.autoCorrect = () => { };
    options.events.calculateDocumentVariable = () => { };
    options.events.characterPropertiesChanged = () => { };
    options.events.contentInserted = () => { };
    options.events.contentRemoved = () => { };
    options.events.documentChanged = () => { };
    options.events.documentFormatted = () => { };
    options.events.documentLoaded = () => { };
    options.events.gotFocus = () => { };
    options.events.hyperlinkClick = () => { };
    options.events.keyDown = () => { };
    options.events.keyUp = () => { };
    options.events.paragraphPropertiesChanged = () => { };
    options.events.lostFocus = () => { };
    options.events.pointerDown = () => { };
    options.events.pointerUp = () => { };
    options.events.saving = () => { };
    options.events.saved = () => { };
    options.events.selectionChanged = () => { };
    options.events.customCommandExecuted = (s, e) => {
        switch (e.commandName) {
            case 'insertEmailSignature':
                s.document.insertParagraph(s.document.length);
                s.document.insertText(s.document.length, '_________');
                s.document.insertParagraph(s.document.length);
                s.document.insertText(s.document.length, 'Best regards,');
                s.document.insertParagraph(s.document.length);
                s.document.insertText(s.document.length, 'John Smith');
                s.document.insertParagraph(s.document.length);
                s.document.insertText(s.document.length, 'john@example.com');
                s.document.insertParagraph(s.document.length);
                s.document.insertText(s.document.length, '+1 (818) 844-0000');
                break;
        }
    };

    options.view.viewType = DevExpress.RichEdit.ViewType.PrintLayout;
    options.view.simpleViewSettings.paddings = {
        left: 15,
        top: 15,
        right: 15,
        bottom: 15,
    };

    options.autoCorrect = {
        correctTwoInitialCapitals: true,
        detectUrls: true,
        enableAutomaticNumbering: true,
        replaceTextAsYouType: true,
        caseSensitiveReplacement: false,
        replaceInfoCollection: [
            { replace: "wnwd", with: "well-nourished, well-developed" },
            { replace: "(c)", with: "©" }
        ],
    };
    // capitalize the first letter at the beginning of a new sentence/line
    options.events.autoCorrect = function (s, e) {
        if (e.text.length == 1 && /\w/.test(e.text)) {
            var prevText = s.document.getText(new DevExpress.RichEdit.Interval(e.interval.start - 2, 2));
            if (prevText.length == 0 || /^(\. |\? |\! )$/.test(prevText) || prevText.charCodeAt(1) == 13) {
                var newText = e.text.toUpperCase();
                if (newText != e.text) {
                    s.beginUpdate();
                    s.history.beginTransaction();
                    s.document.deleteText(e.interval);
                    s.document.insertText(e.interval.start, newText);
                    s.history.endTransaction();
                    s.endUpdate();
                    e.handled = true;
                }
            }
        }
    };

    options.exportUrl = exportUrl;

    options.readOnly = false;
    options.width = '720px';
    options.height = '450px';

    options.spellCheck.enabled = true;
    options.spellCheck.suggestionCount = 10;
    options.spellCheck.checkWordSpelling = function (word, callback) {
        //debugger;
        if (!spellCheckerWorker) {
            var myDictionary = JSON.parse(localStorage.getItem('myDictionary')) || [];
            spellCheckerWorker = new Worker('./spell-checker-worker.js');
            myDictionary.forEach(function (word) {
                spellCheckerWorker.postMessage({
                    command: 'addWord',
                    word: word,
                });
            });

            spellCheckerWorker.onmessage = function (e) {
                var savedCallback = spellCheckerCallbacks[e.data.id];
                delete spellCheckerCallbacks[e.data.id];
                savedCallback(e.data.isCorrect, e.data.suggestions);
            };
        }

        var currId = spellCheckerWorkerCommandId++;
        spellCheckerCallbacks[currId] = callback;
        spellCheckerWorker.postMessage({
            command: 'checkWord',
            word: word,
            id: currId,
        });
    };
    options.spellCheck.addWordToDictionary = function (word) {
        var myDictionary = JSON.parse(localStorage.getItem('myDictionary')) || [];
        myDictionary.push(word);
        localStorage.setItem('myDictionary', JSON.stringify(myDictionary));

        spellCheckerWorker.postMessage({
            command: 'addWord',
            word: word,
        });
    };

    var richElement = document.getElementById("rich-container");

    var richEdit = DevExpress.RichEdit.create(richElement, options);

    var documentAsBase64 = "e1xydGYxXGRlZmYwe1xmb250dGJse1xmMCBDYWxpYnJpO319e1xjb2xvcnRibCA7XHJlZDB"
        + "cZ3JlZW4wXGJsdWUyNTUgO1xyZWQyNTVcZ3JlZW4yNTVcYmx1ZTI1NSA7fXtcKlxkZWZjaHAgXGZzMjJ9e1xzdHl"
        + "sZXNoZWV0IHtccWxcZnMyMiBOb3JtYWw7fXtcKlxjczFcZnMyMiBEZWZhdWx0IFBhcmFncmFwaCBGb250O317XCp"
        + "cY3MyXGZzMjJcY2YxIEh5cGVybGluazt9e1wqXHRzM1x0c3Jvd2RcZnMyMlxxbFx0c3ZlcnRhbHRcdHNjZWxsY2J"
        + "wYXQyXHRzY2VsbHBjdDBcY2x0eGxydGIgTm9ybWFsIFRhYmxlO319e1wqXGxpc3RvdmVycmlkZXRhYmxlfXtcaW5"
        + "mb31cbm91aWNvbXBhdFxzcGx5dHduaW5lXGh0bWF1dHNwXGV4cHNocnRuXHNwbHRwZ3BhclxkZWZ0YWI3MjBcc2V"
        + "jdGRcbWFyZ2xzeG4xNDQwXG1hcmdyc3huMTQ0MFxtYXJndHN4bjE0NDBcbWFyZ2JzeG4xNDQwXGhlYWRlcnk3MjB"
        + "cZm9vdGVyeTcyMFxwZ3dzeG4xMjI0MFxwZ2hzeG4xNTg0MFxjb2xzMVxjb2xzeDcyMFxwYXJkXHBsYWluXHFse1x"
        + "mczIyXGNmMFxjczEgRG9jdW1lbnQgdGV4dH1cZnMyMlxjZjBccGFyfQ==";
    richEdit.openDocument(documentAsBase64, 'DocumentName', DevExpress.RichEdit.DocumentFormat.Rtf);
}