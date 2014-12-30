/********************************************************
 *
 * Custom Javascript code 
 *
 *******************************************************/
$(document).ready(function() {

  //IE placeholders
  $('[placeholder]').focus(function() {
    var input = $(this);
    if (input.val() == input.attr('placeholder')) {
      if (this.originalType) {
        this.type = this.originalType;
        delete this.originalType;
      }
      input.val('');
      input.removeClass('placeholder');
    }
  }).blur(function() {
    var input = $(this);
    if (input.val() == '') {
      input.addClass('placeholder');
      input.val(input.attr('placeholder'));
    }
  }).blur();  
  
  //Bootstrap tooltip
  // invoke by adding _tooltip to a tags (this makes it validate)
  $('body').tooltip({
    selector: "a[class*=_tooltip]"
  });
    
  //Bootstrap popover
  // invoke by adding _popover to a tags (this makes it validate)
  $('body').popover({
    selector: "a[class*=_popover]",
    trigger: "hover"
  });
  
  //show hide elements
  $('.show-hide').each(function() {
    $(this).click(function() {
      var state = 'open'; //assume target is closed & needs opening
      var target = $(this).attr('data-target');
      var targetState = $(this).attr('data-target-state');
      
      //allows trigger link to say target is open & should be closed
      if (typeof targetState !== 'undefined' && targetState !== false) {
        state = targetState;
      }
      
      if (state == 'undefined') {
        state = 'open';
      }
      
      $(target).toggleClass('show-hide-'+ state);
      $(this).toggleClass(state);      
    });
  });
   
});