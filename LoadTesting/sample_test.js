import http from 'k6/http';

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,   
    vus: 1,
    duration: '10s'
};

export default () => {
    http.get('http://http://localhost:5000/benchmark/ok/123')
};