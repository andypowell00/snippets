<?php
function grabPosts($post_status = "publish", $post_type = "post",$posts_per_page="-1",
	$orderby=false,$order=false,$meta_val=false,$meta_key =false)
{//queries posts and returns the query
	
$args=array(

  'post-status' => $post_status,
  'post_type' => $post_type,
  'posts_per_page' => $posts_per_page

);
if($order != false){

	$args['order'] = $order;
}
if($orderby != false){
	$args['orderby'] = $orderby;
}
if($meta_key != false){

	$args['meta_value'] = $meta_val;
	$args['meta_key'] = $meta_key;
}
 
$my_query = null;
$my_query = new WP_Query($args);
return $my_query;
}//end grabPosts()
add_action('grabPosts','theme_scripts');
add_action('arrayLeaders','theme_scripts');
class leader{
public $name;
public $linkedin;
public $location;
public $jobtitle;
public $cat;
public $thumbURL;
}
function arrayLeaders($meta_v)
{//queries posts and puts into an array of custom objects , using the example class above
  $arrayLeaders = array();
  $leaders = grabPosts($post_status = "publish", $post_type = "leadership",$posts_per_page="-1",$orderby=false,$order=false,$meta_val=$meta_v,$meta_key="category");
if( $leaders ->have_posts() ) {
					
while ($leaders ->have_posts()) : $leaders ->the_post(); 
	$leadObj = new leader();
	$id = get_the_ID();
	$leadObj->id = $id;
	
	$leadObj->name =get_the_title($id);
	$leadObj->location = get_post_meta($id,'location',true);
	$leadObj->jobTitle = get_post_meta($id,'title',true);
	$leadObj->linkedin = get_post_meta($id,'linkedin',true);
	
	$imgID = get_post_meta($id,'image',true);
	$leadObj->thumbURL = get_post_meta($imgID,'_wp_attached_file',true);
	
	$arrayLeaders[] = $leadObj; 										
endwhile;}  //end if 
return $arrayLeaders;
}//end arrayLeaders()
function grabWeather($city)
{//takes in a city and grabs weather info from OpenWeatherMap
$cSession = curl_init(); 
$weatherObj = new weatherHolder();
curl_setopt($cSession,CURLOPT_URL,"http://api.openweathermap.org/data/2.5/weather?id=".$city."&units=imperial&APPID=yourappidgoeshere");

curl_setopt($cSession,CURLOPT_RETURNTRANSFER,true);
curl_setopt($cSession,CURLOPT_HEADER, false);
curl_setopt($cSession, CURLOPT_CONNECTTIMEOUT ,2); 
curl_setopt($cSession, CURLOPT_TIMEOUT, 1); 
try{
$result = curl_exec($cSession);


$json = json_decode($result);

if(!empty($json)){
$desc= $json->weather[0]->main;  //current weather conditions string

$res = $json->main->{'temp'}; //current temp


$weatherObj->temp = $res;
$weatherObj->cond = $desc;

curl_close($cSession);
return $weatherObj;
}
else{return $weatherObj;}
}catch (Exception $e) {
	
	curl_close($cSession);
	return $weatherObj;
}
}
add_action('grabWeather','theme_scripts');
?>