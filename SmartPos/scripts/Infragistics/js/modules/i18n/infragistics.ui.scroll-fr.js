﻿/*!@license
* Infragistics.Web.ClientUI Scroll localization resources 16.2.20162.2141
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

    if (!$.ig.Scroll) {
	    $.ig.Scroll = {};

	    $.extend($.ig.Scroll, {
		    locale: {
		        errorNoElementLink: "L'élément en cours de liaison n'existe pas.",
		        errorNoScrollbarLink: "L'élément de la barre de défilement en cours de liaison n'existe pas."
		    }
	    });

    }
}));// REMOVE_FROM_COMBINED_FILES
