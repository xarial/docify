function toggle(span, folder) {            
	if (folder && folder.classList && folder.classList.contains('in')) {
		span.innerHTML = "&#9656;";
		folder.classList.remove("in");
		}
	else {
			span.innerHTML = "&#9662;";
			folder.classList.add("in");
		}	
	}

function findParentNav (el) {
		while ((el = el.parentElement) && el.nodeName != "UL" && !el.classList.contains('toc-container'));
		return el;
	}

function expandNode(toc, url) {
	var elems = toc.getElementsByClassName("tocEntry");
	
	for (var i = 0; i < elems.length; i++) {
		var link = elems[i];
		if(link.href === url){
			if(!link.classList.contains('active')) {
				link.classList.add("active");
			}
			
			var childSpans = link.parentElement.getElementsByClassName("expbutton");

			if(childSpans && childSpans.length == 1){
				var span = childSpans[0];
				var folderId = span.id.toString().substring(0, span.id.toString().length - "Span".length);
				var folder = document.getElementById(folderId);
				toggle(span, folder);
			}
			
			var folder = findParentNav(link);
			while(folder){
				var spanId = folder.id + "Span";
				var span = document.getElementById(spanId);
				toggle(span, folder);
				folder = findParentNav(folder);
			}
			break;
		}
	  }
}