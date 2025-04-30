import {
    loginUser,
    selectUserError,
    selectUserLoading,
} from 'store/user.slice';
import styles from './auth.module.scss';
import { NavLink, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'store/store';
import { useState } from 'react';

export function Login() {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        login: '',
        password: '',
    });
    const error = useSelector(selectUserError);
    const isLoading = useSelector(selectUserLoading);
    const [fieldErrors, setFieldErrors] = useState({
        login: '',
        password: '',
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData((prev) => ({
            ...prev,
            [name]: value,
        }));

        if (fieldErrors[name as keyof typeof fieldErrors]) {
            setFieldErrors((prev) => ({
                ...prev,
                [name]: '',
            }));
        }
    };

    const validateForm = () => {
        const newErrors = {
            login: '',
            password: '',
        };
        let isValid = true;

        if (!formData.login.trim()) {
            newErrors.login = 'Поле не может быть пустым';
            isValid = false;
        }

        if (!formData.password) {
            newErrors.password = 'Поле не может быть пустым';
            isValid = false;
        }

        setFieldErrors(newErrors);
        return isValid;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!validateForm()) return;

        const result = await dispatch(
            loginUser({
                login: formData.login,
                password: formData.password,
            })
        );

        if (loginUser.fulfilled.match(result)) {
            navigate('/profile');
        }
    };

    return (
        <div className={styles.mainContainer}>
            <div className={styles.container}>
                <div className={styles.header}>
                    <h1 className={styles.title}>Вход</h1>
                    <h2 className={styles.subtitle}>
                        Рады, что Вы снова с нами!
                    </h2>
                </div>
                <form onSubmit={handleSubmit} className={styles.form}>
                    <div className={styles.inputContainer}>
                        <input
                            name='login'
                            autoComplete='off'
                            type='text'
                            placeholder='Логин'
                            className={styles.input}
                            value={formData.login}
                            onChange={handleChange}
                        />
                        <span className={`${styles.email} ${styles.icon}`} />
                        {fieldErrors.login && (
                            <p className={styles.error_text}>
                                <span className={styles.error} />
                                {fieldErrors.login}
                            </p>
                        )}
                    </div>
                    <div className={styles.inputContainer}>
                        <input
                            name='password'
                            autoComplete='off'
                            type='password'
                            placeholder='Пароль'
                            className={styles.input}
                            value={formData.password}
                            onChange={handleChange}
                        />
                        <span className={`${styles.lock} ${styles.icon}`} />
                        {fieldErrors.password && (
                            <p className={styles.error_text}>
                                <span className={styles.error} />
                                {fieldErrors.password}
                            </p>
                        )}
                    </div>
                    {error && (
                        <div className={styles.inputContainer}>
                            <p className={styles.error_text}>
                                <span className={styles.error} />
                                {error}
                            </p>
                        </div>
                    )}
                    <button
                        type='submit'
                        className={styles.button}
                        disabled={isLoading}
                    >
                        {isLoading ? 'Загрузка...' : 'Войти'}
                    </button>
                </form>
                <div className={styles.buttons}>
                    <p>
                        Забыли пароль?{' '}
                        <NavLink to='/pass-recovery' className={styles.link}>
                            Восстановить пароль
                        </NavLink>
                    </p>
                    <p>
                        Нет аккаунта?{' '}
                        <NavLink to='/register' className={styles.link}>
                            Зарегистрироваться
                        </NavLink>
                    </p>
                </div>
            </div>
        </div>
    );
}
