<?php  //defining a custom post types & customizing admin menu
function custom_init() {
	$labels = array(
		'name'               => __( 'Custom' ),
		'singular_name'      => __( 'Custom'  ),
		'menu_name'          => __( 'Custom'),
		'name_admin_bar'     => __( 'Custom' ),
		'add_new'            => __( 'Add New' ),
		'add_new_item'       => __( 'Add New Custom' ),
		'new_item'           => __( 'New Custom' ),
		'edit_item'          => __( 'Edit Custom' ),
		'view_item'          => __( 'View Custom' ),
		'all_items'          => __( 'All Customs' ),
		'search_items'       => __( 'Search Customs' ),
		'not_found'          => __( 'No customs found.' ),
		'not_found_in_trash' => __( 'No customs found in Trash.' )
	);

    $args = array(
      'label' => 'Custom',
       'labels' => $labels,
        'public' => true,
        'show_ui' => true,
        'capability_type' => 'post',
        'hierarchical' => false,
        'rewrite' => array('slug' => 'custom'),
        'query_var' => true,
        'menu_icon' => 'dashicons-businessman', //swap out depending on type
        'supports' => array(
            'title',
            'editor',
            'excerpt',
            'trackbacks',
            'custom-fields',
            'comments',
            'revisions',
            'thumbnail',
            'author',
            'page-attributes',)
        );
    register_post_type( 'custom', $args );
}
add_action( 'init', 'custom_init' );
function change_post_menu_label() {
    global $menu, $submenu;
    //changing menu copy of 'Posts' default to 'Articles'
    $menu[5][0] = 'Articles';
    $submenu['edit.php'][5][0] = 'Article';
    $submenu['edit.php'][10][0] = 'New Article';
    $submenu['edit.php'][16][0] = 'Article Tags';
    echo '';
}
add_action( 'admin_menu', 'change_post_menu_label' );
function change_post_object_label() {  //renaming posts -> articles in admin 
    global $wp_post_types;

    $labels = &$wp_post_types['post']->labels;
    $labels->name = 'Article';
    $labels->singular_name = 'Article';
    $labels->add_new = 'New Article';
    $labels->menu_name = 'Article';
    $labels->name_admin_bar = 'Articles';
    $labels->add_new_item = 'New Article';
    $labels->edit_item = 'Edit Article';
    $labels->new_item = 'New Article';
    $labels->view_item = 'View Article';
    $labels->search_items = 'Search Article';
    $labels->not_found = 'Not found';
    $labels->not_found_in_trash = 'Not found in trash';
}
add_action( 'admin_menu', 'change_post_object_label' );
add_action( 'wp_before_admin_bar_render', function() {
global $wp_admin_bar;
$wp_admin_bar->remove_menu('wp-logo');  
$wp_admin_bar->remove_menu('comments');
}, 7 );
add_action('login_head','new_login_styles');
function new_login_styles() {  //customizing wordpress login screen w/ new logo & link url
    echo '<style type="text/css">
        .login h1 a {
        background-image:url('.get_bloginfo( 'template_url' ).'/img/logo--dark.png) !important;
        background-size:184px;
        width:184px;
        height:35px;
          }
    </style>';
}
add_filter('login_headertitle', 'new_login_title');
add_filter('login_headerurl', 'new_login_url');
function new_login_url() {
    return home_url( '/' );
}
function new_login_title() {
    return get_option('blogname');
}
if (! function_exists('dashboard_footer') ){
function dashboard_footer () {
echo ''; //hiding wordpress shout out in admin footer
}
}
add_filter('admin_footer_text', 'dashboard_footer');
function remove_reorderblurb() { //hiding re order posts plugin garbage paragraph
   echo '<style type="text/css">
           #cpt_info_box{display:none;}
         </style>';
}
add_action('admin_head', 'remove_reorderblurb');
function custom_menu_order($menu_ord) {
    if (!$menu_ord) return true;
 
    return array(
      'index.php', // Dashboard
      'edit.php', // Posts
      'edit.php?post_type=client', // Custom type one
      'edit.php?post_type=leadership', // Custom type two
      'edit.php?post_type=weather', // Custom type three
      'edit.php?post_type=location', // Custom type four
      'edit.php?post_type=copy',
      'edit.php?post_type=meta',
      'edit.php?post_type=aptitude',
      'theme-editor.php',  //FIX THIS LINK
      'separator1', // First separator
      'edit.php?post_type=page', // Pages
      'upload.php', // Medi
      'separator2', // Second separator
      'themes.php', // Appearance
      'plugins.php', // Plugins
      'users.php', // Users
      'tools.php', // Tools
      'options-general.php', // Settings
      'separator-last', // Last separator
      'edit.php?post_type=acf', // Custom type five
    );
  }
  add_filter('custom_menu_order', 'custom_menu_order'); // Activate custom_menu_order
  add_filter('menu_order', 'custom_menu_order');
  function editphp_enqueue_loc($hook){
    if ($hook == 'post.php' || $hook == 'post-new.php' || $hook == 'edit.php') {
        global $current_screen;
         if ('weather' != $current_screen->post_type) {return;}
        wp_register_script( 
            'readonly_script', 
            getRootUrl() . 'wp-content/themes/digital/js/admin.locations.js', 
            array( 'jquery' )
        );
        wp_enqueue_script( 'readonly_script' );
    }
  }
  add_action('admin_enqueue_scripts','editphp_enqueue_loc');
   function editor_enqueue($hook){
    if ($hook == 'theme-editor.php') {
        
        wp_register_script( 
            'readonly_script', 
            getRootUrl() . 'wp-content/themes/digital/js/admin.editor.js', 
            array( 'jquery' )
        );
        wp_enqueue_script( 'readonly_script' );
    }
  }
  add_action('admin_enqueue_scripts','editor_enqueue');
  function add_exportButton(){
  //adding in a csv export to a specific section of the admin.
        global $current_screen;
        
     if ('aptitude' != $current_screen->post_type) {return;}
     ?>
     <script type="text/javascript">
            jQuery(document).ready( function($)
            {
                function exportcsv(){
                    var post_url = location.origin + '/csv';
                    window.location = post_url;
                }
                jQuery(jQuery(".wrap h2")[0]).append("<a style='cursor: pointer;' id='doc_popup' class='add-new-h2'>Export</a>");
                $('#doc_popup').click(function(){

                        exportcsv();
                });

            });
        </script>
     <?php
  }
  add_action('admin_head-edit.php','add_exportButton');
  function remove_upgrade_nag() {
   echo '<style type="text/css">
           .update-nag {display: none}
         </style>';
}
add_action('admin_head', 'remove_upgrade_nag');
function limit_admin_menu(){
$current_user = wp_get_current_user();  //quick and dirty show/hide based on user_login
    if($current_user->user_login === 'Role1' || $current_user->user_login === 'Role2' ){
  //hide things
        ?>
        <script type="text/javascript">

            jQuery(document).ready( function($)
            {
                $('#wp-admin-bar-new-content').hide();
                $('#dashboard-widgets-wrap').hide();
                //$('.meta-box-sortables').hide();
                $(".wp-menu-name").each(function()
                {

                    if($(this).text() !== 'The text you want'){

                       $(this).parent().parent().hide();
                    }                  
                });

            });
        </script>
        <?php

}//end if 
}
add_action('admin_head','limit_admin_menu');
  function admin_bar_theme_editor_option() { 
    global $wp_admin_bar;  

        if ( !is_super_admin() || !is_admin_bar_showing() )     

        return;   

        $wp_admin_bar->add_menu(       

            array( 'id' => 'edit-theme',           

            'title' => __('Editor'),

            'href' => admin_url( 'theme-editor.php')      

        )   

        );

    }
add_action( 'admin_bar_menu', 'admin_bar_theme_editor_option', 100 );


