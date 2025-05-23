import { Modal } from 'components/common';
import styles from './edit-profile.module.scss';
import { ReactComponent as CheckIcon } from '../../../assets/svg/check_black.svg';
import { ReactComponent as CloseIcon } from '../../../assets/svg/cancel.svg';
import { User } from 'types';
import default_photo from '../../../assets/img/default-user.png';
import { useState } from 'react';
import { useDispatch } from 'store/store';
import { updateUser } from 'store/user.slice';
import { UpdateUserDto, uploadUserImage } from 'store/api';

type props = {
    onClose: () => void;
    user: User;
};

export function EditProfile(props: props) {
    const { user, onClose } = props;
    const [login, setLogin] = useState(user.login);
    const [email, setEmail] = useState(user.email);
    const [phone, setPhone] = useState(user.phone);
    const [newPhoto, setNewPhoto] = useState<File | null>(null);
    const dispatch = useDispatch();

    const handleSave = async () => {
        try {
            let pictureUrl = '';

            if (newPhoto) {
                pictureUrl = await uploadUserImage(newPhoto);
            }

            const data: UpdateUserDto = {};

            if (login && login !== user.login) {
                data.login = login;
            }

            if (email && email !== user.email) {
                data.email = email;
            }

            if (phone && phone !== user.phone) {
                data.phone = phone;
            }

            if (pictureUrl) {
                data.profilePictureUrl = pictureUrl;
            }

            if (Object.keys(data).length === 0) {
                onClose();
                return;
            }

            dispatch(updateUser({ id: user.id, data }))
                .unwrap()
                .then(() => onClose())
                .catch((e) => alert(e));
        } catch (e) {
            alert('Ошибка при обновлении профиля');
        }
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            setNewPhoto(file);
        }
    };

    return (
        <Modal onClose={onClose}>
            <div className={styles.mainContainer}>
                <p className={styles.title}>Редактирование профиля</p>
                <div className={styles.photoContainer}>
                    <img
                        className={styles.photo}
                        src={
                            newPhoto
                                ? URL.createObjectURL(newPhoto)
                                : user.pictureUrl === 'none'
                                ? default_photo
                                : user.pictureUrl
                        }
                        alt='Фото профиля'
                    />
                    <label className={`button ${styles.uploadButton}`}>
                        Загрузить фото
                        <input
                            type='file'
                            accept='image/*'
                            className={styles.inputFileHidden}
                            onChange={handleFileChange}
                        />
                    </label>
                </div>

                <div className={styles.info}>
                    <div className={styles.inputContainer}>
                        <p>Никнейм</p>
                        <input
                            type='text'
                            placeholder='Никнейм'
                            className={styles.input}
                            value={login}
                            onChange={(e) => setLogin(e.target.value)}
                            maxLength={25}
                        />
                    </div>
                    <div className={styles.inputContainer}>
                        <p>Контактная информация</p>
                        <input
                            type='text'
                            placeholder='Email'
                            className={styles.input}
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />
                        <input
                            type='text'
                            placeholder='Номер телефона'
                            className={styles.input}
                            value={phone}
                            onChange={(e) => setPhone(e.target.value)}
                        />
                    </div>
                </div>
                <div className={styles.buttons}>
                    <button
                        className={`${styles.button} ${styles.admit}`}
                        onClick={handleSave}
                    >
                        <CheckIcon className={styles.check} />
                        Подтвердить
                    </button>
                    <button
                        className={`${styles.button} ${styles.close}`}
                        onClick={onClose}
                    >
                        <CloseIcon />
                        Отменить
                    </button>
                </div>
            </div>
        </Modal>
    );
}
