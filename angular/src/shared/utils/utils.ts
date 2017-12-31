
export function expiredJwt(token: string) {
  let base64Url = token.split('.')[1];
  let base64 = base64Url.replace('-', '+').replace('_', '/');
  let jwt = JSON.parse(window.atob(base64));
  let exp = jwt.exp * 1000;
  let currentTime = new Date().getTime() / 1000;
  if (currentTime > jwt.exp) {
    return true;
  } else {
    return false;
  }
}

export function checkOptions(options?: any) {
  if (options !== undefined) {
    if (options.useAuth) {
      return true;
    }
  } else {
    return false;
  }
}


export function xwwwfurlenc(srcjson){
	if(typeof srcjson !== "object") if(typeof console !== "undefined"){ console.log("\"srcjson\" is not a JSON object"); return null; }
	let u = encodeURIComponent;
	var urljson = "";
	var keys = Object.keys(srcjson);
	for(var i=0; i <keys.length; i++){
		urljson += u(keys[i]) + "=" + u(srcjson[keys[i]]);
		if(i < (keys.length-1))urljson+="&";
	}
	return urljson;
}

//Will only decode as strings
//Without embedding extra information, there is no clean way to know what type of variable it was.
export function dexwwwfurlenc(urljson){
	var dstjson = {};
	var ret;
	var reg = /(?:^|&)(\w+)=(\w+)/g;
	while((ret = reg.exec(urljson)) !== null){
		dstjson[ret[1]] = ret[2];
	}
	return dstjson;
}