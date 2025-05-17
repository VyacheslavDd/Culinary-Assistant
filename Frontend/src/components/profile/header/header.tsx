/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { LayoutHeader } from 'components/layout';
import styles from './header.module.scss';
import edit from '../../../assets/svg/edit.svg';
import exit from '../../../assets/svg/exit.svg';
import { logoutUser, selectUser } from 'store/user.slice';
import { useDispatch, useSelector } from 'store/store';
import { useNavigate } from 'react-router-dom';
import default_user from '../../../assets/img/default-user.png';
import { EditProfile } from '../edit-profile/edit-profile';
import { useState } from 'react';

export function HeaderProfile() {
    const user = useSelector(selectUser);
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [open, setOpen] = useState(false);

    const handleCloseModal = () => {
        setOpen(false);
    };

    const { login, email, phone, pictureUrl } = user!;

    const handleLogout = () => {
        dispatch(logoutUser());
        navigate('/');
    };

    return (
        <div className={styles.mainContainer}>
            <LayoutHeader>
                <div className={styles.container}>
                    <img
                        src={pictureUrl === 'none' ? default_user : pictureUrl}
                        alt='profile'
                        className={styles.img}
                    />
                    <div className={styles.info}>
                        <div className={styles.name}>
                            <h2 className={styles.title}>{login}</h2>
                            <div className={styles.buttons}>
                                <button
                                    className={styles.button}
                                    onClick={() => setOpen(true)}
                                >
                                    <img src={edit} alt='edit' />
                                    Изм.
                                </button>
                                <button
                                    className={`${styles.button} ${styles.exit}`}
                                    onClick={handleLogout}
                                >
                                    <img src={exit} alt='exit' />
                                    Выйти
                                </button>
                            </div>
                        </div>
                        <div className={styles.description}>
                            {/* <div className={styles.item}>
                                <div className={styles.textContainer}>
                                    <p className={styles.text}>Пол:</p>
                                    <p className={styles.text}>
                                        Дата рождения:
                                    </p>
                                </div>
                                <div className={styles.textContainer}>
                                    <p className={styles.text}>Женский</p>
                                    <p className={styles.text}>19.05.2004</p>
                                </div>
                            </div> */}
                            <div className={styles.item}>
                                <div className={styles.textContainer}>
                                    <p className={styles.text}>Emai:</p>
                                    <p className={styles.text}>
                                        Номер телефона:
                                    </p>
                                </div>
                                <div className={styles.textContainer}>
                                    <p className={styles.text}>
                                        {email || 'Не указано'}
                                    </p>
                                    <p className={styles.text}>
                                        {phone || 'Не указано'}
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </LayoutHeader>
            {open ? (
                <EditProfile onClose={handleCloseModal} user={user!} />
            ) : (
                <></>
            )}
        </div>
    );
}
