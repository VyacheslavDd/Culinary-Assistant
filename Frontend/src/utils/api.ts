import { setCookie, getCookie, deleteCookie } from './cookie';

const URL = process.env.REACT_APP_BURGER_API_URL;

const checkResponse = <T>(res: Response): Promise<T> =>
  res.ok ? res.json() : res.json().then((err) => Promise.reject(err));

