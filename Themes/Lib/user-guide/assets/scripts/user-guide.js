/*	window.onload = function() {
		updateLayout();
	};
	function updateLayout() {
	if(window.innerWidth < 600){
		var tocExpander = document.getElementById("tocExpander");
		tocExpander.removeAttribute("hidden");
		var toc = document.createElement("div");
		tocExpander.appendChild(toc); 
		createToc(toc);
		}
		else {
			createToc(document.getElementById("toc"));
		}
	}
	function createToc(toc){
		var activeUrl = '{{ site.url }}{{ page.url }}';
		renderToc(toc);
		expandNode(toc, activeUrl);
	}*/