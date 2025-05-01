import { LayoutHeader } from 'components/layout';
import styles from './header.module.scss';
import profile_photo from '../../../assets/img/profile_photo.png';
import edit from '../../../assets/svg/edit.svg';
import exit from '../../../assets/svg/exit.svg';
import { logoutUser, selectUser } from 'store/user.slice';
import { useDispatch, useSelector } from 'store/store';
import { useNavigate } from 'react-router-dom';

export function HeaderProfile() {
    const user = useSelector(selectUser);
    const dispatch = useDispatch();
    const navigate = useNavigate();
    // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
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
                        src={profile_photo}
                        alt='profile'
                        className={styles.img}
                    />
                    <div className={styles.info}>
                        <div className={styles.name}>
                            <h2 className={styles.title}>{login}</h2>
                            <div className={styles.buttons}>
                                <button className={styles.button}>
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
        </div>
    );
}
