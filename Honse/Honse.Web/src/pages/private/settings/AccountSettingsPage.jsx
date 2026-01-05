import { useEffect, useMemo, useRef, useState } from "react";
import axios from "axios";
import "./AccountSettingsPage.css";

const RAW_BASE = process.env.REACT_APP_API_URL || "https://localhost:2000";

const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const usernameRegex = /^[a-zA-Z0-9._]+$/;

export default function AccountSettingsPage() {
    const hydratedRef = useRef(false);

    const [profile, setProfile] = useState({ username: "", email: "" });
    const [passwords, setPasswords] = useState({
        currentPassword: "",
        newPassword: "",
        confirmNewPassword: "",
    });

    const [status, setStatus] = useState({ type: "", message: "" });
    const [loading, setLoading] = useState({ initial: true, profile: false, password: false });

    const [touched, setTouched] = useState({
        username: false,
        email: false,
        currentPassword: false,
        newPassword: false,
        confirmNewPassword: false,
    });

    const [errors, setErrors] = useState({
        username: "",
        email: "",
        currentPassword: "",
        newPassword: "",
        confirmNewPassword: "",
    });

    const setErrorBanner = (message) => setStatus({ type: "error", message });
    const setSuccessBanner = (message) => setStatus({ type: "success", message });

    function validateProfile(nextProfile) {
        const e = { username: "", email: "" };

        const u = (nextProfile.username || "").trim();
        const em = (nextProfile.email || "").trim();

        if (!u) e.username = "Username is required.";
        else if (u.length < 3) e.username = "Username must be at least 3 characters.";
        else if (u.length > 20) e.username = "Username must be at most 20 characters.";
        else if (/\s/.test(u)) e.username = "Username cannot contain spaces.";
        else if (!usernameRegex.test(u)) e.username = "Use only letters, numbers, dot, underscore.";

        if (!em) e.email = "Email is required.";
        else if (!emailRegex.test(em)) e.email = "Enter a valid email address.";

        return e;
    }

    function validatePasswords(nextPasswords) {
        const e = { currentPassword: "", newPassword: "", confirmNewPassword: "" };

        const cur = nextPasswords.currentPassword || "";
        const np = nextPasswords.newPassword || "";
        const cnp = nextPasswords.confirmNewPassword || "";

        if (!cur) e.currentPassword = "Current password is required.";

        if (!np) e.newPassword = "New password is required.";
        else if (np.length < 8) e.newPassword = "New password must be at least 8 characters.";

        if (!cnp) e.confirmNewPassword = "Please confirm the new password.";
        else if (np !== cnp) e.confirmNewPassword = "Passwords do not match.";

        return e;
    }

    const profileErrors = useMemo(() => validateProfile(profile), [profile]);
    const passwordErrors = useMemo(() => validatePasswords(passwords), [passwords]);

    const isProfileValid = !profileErrors.username && !profileErrors.email;
    const isPasswordValid =
        !passwordErrors.currentPassword && !passwordErrors.newPassword && !passwordErrors.confirmNewPassword;

    useEffect(() => {
        setErrors((prev) => ({
            ...prev,
            username: profileErrors.username,
            email: profileErrors.email,
        }));
    }, [profileErrors]);

    useEffect(() => {
        setErrors((prev) => ({
            ...prev,
            currentPassword: passwordErrors.currentPassword,
            newPassword: passwordErrors.newPassword,
            confirmNewPassword: passwordErrors.confirmNewPassword,
        }));
    }, [passwordErrors]);

    useEffect(() => {
        if (hydratedRef.current) return;

        (async () => {
            setStatus({ type: "", message: "" });

            try {
                const token = localStorage.getItem("token");
                if (!token) {
                    setErrorBanner("No token found. Please log in again.");
                    return;
                }

                const res = await axios.get(`${RAW_BASE}/api/users/me`, {
                    headers: { Authorization: `Bearer ${token}` },
                });

                const me = res.data;

                setProfile({
                    username: (me.username ?? me.userName ?? me.UserName ?? "").trim(),
                    email: (me.email ?? me.Email ?? "").trim(),
                });

                hydratedRef.current = true;
            } catch (e) {
                console.error("ME ERROR:", e?.response?.status, e?.response?.data, e?.message);
                setErrorBanner("Failed to load your account details.");
            } finally {
                setLoading((s) => ({ ...s, initial: false }));
            }
        })();
    }, []);

    function markTouched(field) {
        setTouched((t) => ({ ...t, [field]: true }));
    }

    async function handleSaveProfile(e) {
        e.preventDefault();
        setStatus({ type: "", message: "" });

        setTouched((t) => ({ ...t, username: true, email: true }));

        const eProfile = validateProfile(profile);
        if (eProfile.username || eProfile.email) return;

        setLoading((s) => ({ ...s, profile: true }));
        try {
            // TODO: wire backend PUT
            // const token = localStorage.getItem("token");
            // await axios.put(`${RAW_BASE}/api/users/me`, profile, {
            //   headers: { Authorization: `Bearer ${token}` },
            // });

            setSuccessBanner("Profile updated successfully.");
        } catch {
            setErrorBanner("Failed to update profile.");
        } finally {
            setLoading((s) => ({ ...s, profile: false }));
        }
    }

    async function handleChangePassword(e) {
        e.preventDefault();
        setStatus({ type: "", message: "" });

        setTouched((t) => ({
            ...t,
            currentPassword: true,
            newPassword: true,
            confirmNewPassword: true,
        }));

        const ePw = validatePasswords(passwords);
        if (ePw.currentPassword || ePw.newPassword || ePw.confirmNewPassword) return;

        setLoading((s) => ({ ...s, password: true }));
        try {
            // TODO: wire backend endpoint
            // const token = localStorage.getItem("token");
            // await axios.post(`${RAW_BASE}/api/users/change-password`, {
            //   currentPassword: passwords.currentPassword,
            //   newPassword: passwords.newPassword,
            // }, { headers: { Authorization: `Bearer ${token}` } });

            setPasswords({ currentPassword: "", newPassword: "", confirmNewPassword: "" });
            setTouched((t) => ({
                ...t,
                currentPassword: false,
                newPassword: false,
                confirmNewPassword: false,
            }));

            setSuccessBanner("Password updated successfully.");
        } catch {
            setErrorBanner("Failed to change password.");
        } finally {
            setLoading((s) => ({ ...s, password: false }));
        }
    }

    if (loading.initial) {
        return (
            <div className="settings-wrap">
                <div className="settings-container">
                    <div className="settings-card">
                        <h2 className="settings-card-title">Loading...</h2>
                        <p className="settings-card-sub">Fetching your account details.</p>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="settings-wrap">
            <div className="settings-container">
                <header className="settings-header">
                    <h1 className="settings-title">Account Settings</h1>
                    <p className="settings-sub">Manage your profile and security settings.</p>
                </header>

                {status.message && (
                    <div className={["alert", status.type === "success" ? "alert-success" : "alert-error"].join(" ")}>
                        {status.message}
                    </div>
                )}

                <div className="settings-grid">
                    <section className="settings-card">
                        <h2 className="settings-card-title">Profile</h2>
                        <p className="settings-card-sub">Update your account details.</p>

                        <form onSubmit={handleSaveProfile} className="form-grid" noValidate>
                            <div className="input-row">
                                <label className="label">Username</label>
                                <input
                                    className={["input", touched.username && errors.username ? "input-invalid" : ""].join(" ")}
                                    value={profile.username}
                                    onChange={(e) => setProfile((p) => ({ ...p, username: e.target.value }))}
                                    onBlur={() => markTouched("username")}
                                    placeholder="Username"
                                    autoComplete="username"
                                />
                                {touched.username && errors.username && <div className="field-error">{errors.username}</div>}
                            </div>

                            <div className="input-row">
                                <label className="label">Email</label>
                                <input
                                    type="email"
                                    className={["input", touched.email && errors.email ? "input-invalid" : ""].join(" ")}
                                    value={profile.email}
                                    onChange={(e) => setProfile((p) => ({ ...p, email: e.target.value }))}
                                    onBlur={() => markTouched("email")}
                                    placeholder="Email"
                                    autoComplete="email"
                                />
                                {touched.email && errors.email && <div className="field-error">{errors.email}</div>}
                            </div>

                            <div className="actions">
                                <button className="btn btn-primary" disabled={loading.profile || !isProfileValid}>
                                    {loading.profile ? "Saving..." : "Save changes"}
                                </button>
                            </div>
                        </form>
                    </section>

                    <section className="settings-card">
                        <h2 className="settings-card-title">Security</h2>
                        <p className="settings-card-sub">Change your password.</p>

                        <form onSubmit={handleChangePassword} className="form-grid" noValidate>
                            <div className="input-row">
                                <label className="label">Current password</label>
                                <input
                                    type="password"
                                    className={[
                                        "input",
                                        touched.currentPassword && errors.currentPassword ? "input-invalid" : "",
                                    ].join(" ")}
                                    value={passwords.currentPassword}
                                    onChange={(e) => setPasswords((p) => ({ ...p, currentPassword: e.target.value }))}
                                    onBlur={() => markTouched("currentPassword")}
                                    autoComplete="current-password"
                                />
                                {touched.currentPassword && errors.currentPassword && (
                                    <div className="field-error">{errors.currentPassword}</div>
                                )}
                            </div>

                            <div className="input-row">
                                <label className="label">New password</label>
                                <input
                                    type="password"
                                    className={["input", touched.newPassword && errors.newPassword ? "input-invalid" : ""].join(" ")}
                                    value={passwords.newPassword}
                                    onChange={(e) => setPasswords((p) => ({ ...p, newPassword: e.target.value }))}
                                    onBlur={() => markTouched("newPassword")}
                                    autoComplete="new-password"
                                />
                                {touched.newPassword && errors.newPassword && <div className="field-error">{errors.newPassword}</div>}
                            </div>

                            <div className="input-row">
                                <label className="label">Confirm new password</label>
                                <input
                                    type="password"
                                    className={[
                                        "input",
                                        touched.confirmNewPassword && errors.confirmNewPassword ? "input-invalid" : "",
                                    ].join(" ")}
                                    value={passwords.confirmNewPassword}
                                    onChange={(e) => setPasswords((p) => ({ ...p, confirmNewPassword: e.target.value }))}
                                    onBlur={() => markTouched("confirmNewPassword")}
                                    autoComplete="new-password"
                                />
                                {touched.confirmNewPassword && errors.confirmNewPassword && (
                                    <div className="field-error">{errors.confirmNewPassword}</div>
                                )}
                            </div>

                            <div className="actions">
                                <button className="btn btn-primary" disabled={loading.password || !isPasswordValid}>
                                    {loading.password ? "Updating..." : "Update password"}
                                </button>
                            </div>
                        </form>
                    </section>
                </div>
            </div>
        </div>
    );
}
