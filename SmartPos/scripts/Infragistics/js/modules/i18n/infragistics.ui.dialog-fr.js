﻿/*!@license
* Infragistics.Web.ClientUI Dialog localization resources 16.2.20162.2141
*
* Copyright (c) 2011-2017 Infragistics Inc.
*
* http://www.infragistics.com/
*
*/

(function (factory) {
	if (typeof define === "function" && define.amd) {
		define( [
			"jquery"
		], factory );
	} else {
		factory(jQuery);
	}
}
(function ($) {
    $.ig = $.ig || {};

    if (!$.ig.Dialog) {
	    $.ig.Dialog = {
		    locale: {
			    closeButtonTitle: "Fermer",
			    minimizeButtonTitle: "Minimiser",
			    maximizeButtonTitle: "Maximiser",
			    pinButtonTitle: "Punaiser",
			    unpinButtonTitle: "Dépunaiser",
			    restoreButtonTitle: "Restaurer",
				setOptionError: "Les modifications de temps d'exécution ne sont pas autorisées pour l'option suivante : "
		    }
	    };
    }
}));// REMOVE_FROM_COMBINED_FILES
