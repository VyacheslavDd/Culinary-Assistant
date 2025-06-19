import { useState } from 'react';
import styles from './card-new.module.scss';
import { ReactComponent as CrossIcon } from '../../../../assets/svg/cross.svg';
import { ReactComponent as PlusIcon } from '../../../../assets/svg/button_plus.svg';
import { createCollectionApi } from 'store/api';
import { useSelector } from 'react-redux';
import { fetchUsersCollections, selectUser } from 'store/user.slice';
import { useNavigate } from 'react-router';
import { useDispatch } from 'store/store';

export function CardNew() {
    const [isOpen, setIsOpen] = useState(false);
    const [title, setTitle] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const user = useSelector(selectUser);
    const navigate = useNavigate();
    const dispatch = useDispatch();

    const handleOpen = () => setIsOpen(true);
    const handleClose = () => setIsOpen(false);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setTitle(e.target.value);
    };

    const handleCreate = async () => {
        if (!title.trim()) {
            alert('Введите название подборки');
            return;
        }

        const newCollection = {
            title,
            isPrivate: false,
            color: 'blue',
            userId: user!.id,
        };

        setIsLoading(true);

        try {
            const response = await createCollectionApi(newCollection);
            setTitle('');
            handleClose();
            navigate(`/collection/${response}`);
            dispatch(fetchUsersCollections(user!.id));
        } catch (error) {
            alert(`Ошибка при попытке создания подборки`);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className={styles.mainContainer}>
            <div className={styles.container}>
                {isOpen ? (
                    <div className={styles.content}>
                        <div className={styles.cross} onClick={handleClose}>
                            <CrossIcon className={styles.iconCross} />
                        </div>
                        <div className={styles.inputContainer}>
                            <input
                                type='text'
                                className={styles.input}
                                placeholder='Название подборки'
                                value={title}
                                onChange={handleInputChange}
                            />
                            <button
                                className={`button ${styles.buttonCreate}`}
                                onClick={handleCreate}
                                disabled={isLoading}
                            >
                                {isLoading ? 'Создание...' : 'Создать'}
                            </button>
                        </div>
                    </div>
                ) : (
                    <div className={styles.plusWrapper} onClick={handleOpen}>
                        <PlusIcon className={styles.iconPlus} />
                    </div>
                )}
            </div>
        </div>
    );
}
