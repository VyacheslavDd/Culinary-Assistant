import { NavLink, useNavigate } from 'react-router-dom';
import styles from './auth.module.scss';
import { useDispatch, useSelector } from 'store/store';
import { useState } from 'react';
import {
    registerUser,
    selectUserError,
    selectUserLoading,
} from 'store/user.slice';

export function Register() {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        login: '',
        email: '',
        password: '',
        confirmPassword: '',
    });
    const error = useSelector(selectUserError);
    const isLoading = useSelector(selectUserLoading);
    const [fieldErrors, setFieldErrors] = useState({
        login: '',
        email: '',
        password: '',
        confirmPassword: '',
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
            email: '',
            password: '',
            confirmPassword: '',
        };
        let isValid = true;

        if (!formData.login.trim()) {
            newErrors.login = 'Поле не может быть пустым';
            isValid = false;
        }

        if (!formData.email.trim()) {
            newErrors.email = 'Поле не может быть пустым';
            isValid = false;
        }

        if (!formData.password) {
            newErrors.password = 'Поле не может быть пустым';
            isValid = false;
        } else if (formData.password.length < 6) {
            newErrors.password = 'Пароль должен содержать минимум 6 символов';
            isValid = false;
        }

        if (formData.password !== formData.confirmPassword) {
            newErrors.confirmPassword = 'Пароли не совпадают';
            isValid = false;
        }

        setFieldErrors(newErrors);
        return isValid;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!validateForm()) return;

        const result = await dispatch(
            registerUser({
                login: formData.login,
                emailOrPhone: formData.email,
                password: formData.password,
            })
        );

        if (registerUser.fulfilled.match(result)) {
            navigate('/profile');
        }
    };

    return (
        <div className={styles.mainContainer}>
            <div className={styles.container}>
                <div className={styles.header}>
                    <h1 className={styles.title}>Регистрация</h1>
                    <h2 className={styles.subtitle}>
                        Мы будем очень рады видеть Вас в нашем сообществе!
                    </h2>
                </div>
                <form className={styles.form} onSubmit={handleSubmit}>
                    <div className={styles.inputContainer}>
                        <input
                            name='login'
                            type='text'
                            placeholder='Логин'
                            className={styles.input}
                            value={formData.login}
                            onChange={handleChange}
                        />
                        <span className={`${styles.person} ${styles.icon}`} />
                        {fieldErrors.login && (
                            <p className={styles.error_text}>
                                <span className={styles.error} />
                                {fieldErrors.login}
                            </p>
                        )}
                    </div>
                    <div className={styles.inputContainer}>
                        <input
                            name='email'
                            type='text'
                            placeholder='Email/телефон'
                            className={styles.input}
                            value={formData.email}
                            onChange={handleChange}
                        />
                        <span className={`${styles.email} ${styles.icon}`} />
                        {fieldErrors.email && (
                            <p className={styles.error_text}>
                                <span className={styles.error} />
                                {fieldErrors.email}
                            </p>
                        )}
                    </div>
                    <div className={styles.inputContainer}>
                        <input
                            name='password'
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
                    <div className={styles.inputContainer}>
                        <input
                            name='confirmPassword'
                            type='password'
                            placeholder='Подтвердите пароль'
                            className={styles.input}
                            value={formData.confirmPassword}
                            onChange={handleChange}
                        />
                        <span className={`${styles.lock} ${styles.icon}`} />
                        {fieldErrors.confirmPassword && (
                            <p className={styles.error_text}>
                                <span className={styles.error} />
                                {fieldErrors.confirmPassword}
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
                    <button type='submit' className={styles.button} disabled={isLoading}>
                        Зарегистрироваться
                    </button>
                </form>
                <div className={styles.buttons}>
                    <p>
                        Уже есть аккаунт?{' '}
                        <NavLink to='/login' className={styles.link}>
                            Войти
                        </NavLink>
                    </p>
                </div>
            </div>
        </div>
    );
}
