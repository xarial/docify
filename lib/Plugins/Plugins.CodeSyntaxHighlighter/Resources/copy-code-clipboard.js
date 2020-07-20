window.addEventListener("load", function ()
{
    var cp = new ClipboardJS('.copy-code-btn', {
        target: function (trigger) {
            return trigger.nextElementSibling;
        }
    });
});