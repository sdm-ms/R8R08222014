/// <reference path="jquery-1.3.2-vsdoc2.js" />



var treeSetupAlreadyInitialized = false;
function treeSetup() {
    if (treeSetupAlreadyInitialized)
        return;
    treeSetupAlreadyInitialized = true;
    $('#mytopicstree').tree({
        types: {
            "default" : {
        		clickable	: true,
        		renameable	: false,
        		deletable	: false,
        		creatable	: false,
        		draggable   : false,
        		max_children	: -1,
        		max_depth	: -1,
        		valid_children	: "all",
        		icon : {
        			image : false,
        			position : false
        		}
        	}
        },
        callback: { onchange: handleTreeNodeClick },
        ui: {
            dots: false,
            theme_name: "classic"
        }
    });
    $("[id$='TopicsCell']").show();
}


function handleTreeNodeClick(NODE,TREE_OBJ) {
    window.location = NODE.firstChild;
}

/* will not call if another page uses its own pageLoad -- so add call to treeSetup */
//function pageLoad() {
//    treeSetup();
//}